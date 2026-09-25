using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[DisallowMultipleComponent]
public abstract class NetworkDiscovery<TBroadcast, TResponse> : PersistentSingleton<NetworkDiscovery<TBroadcast, TResponse>>
    where TBroadcast : INetworkSerializable, new()
    where TResponse : INetworkSerializable, new()
{
    private enum MessageType : byte
    {
        Broadcast = 0,
        Response = 1,
    }

    private const int HeaderSizeInBytes = sizeof(long) + sizeof(byte);
    private const int InitialDatagramCapacity = 1024;
    private const int MaxDatagramCapacity = 1024 * 64;

    [SerializeField] private ushort discoveryPort = 47777;

    // long instead of ulong because the Inspector does not support ulong.
    [SerializeField, FormerlySerializedAs("m_UniqueApplicationId")]
    private long uniqueApplicationId;

    private UdpClient udpClient;

    protected ushort DiscoveryPort => discoveryPort;

    protected virtual IPAddress BroadcastAddress => IPAddress.Broadcast;

    public bool IsRunning => udpClient != null;
    public bool IsServer { get; private set; }
    public bool IsClient { get; private set; }

    protected virtual void OnDestroy()
    {
        StopDiscovery();
    }

    private void OnValidate()
    {
        if (uniqueApplicationId != 0) return;

        long highBits = (long)Random.Range(int.MinValue, int.MaxValue) << 32;
        long lowBits = (uint)Random.Range(int.MinValue, int.MaxValue);
        uniqueApplicationId = highBits | lowBits;
    }

    public bool TryStartServerDiscovery() => TryStartDiscovery(isServer: true);

    public bool TryStartClientDiscovery() => TryStartDiscovery(isServer: false);

    public void StopDiscovery()
    {
        IsServer = false;
        IsClient = false;

        UdpClient clientToClose = udpClient;
        udpClient = null;
        clientToClose?.Close();
    }

    public void SendClientBroadcast(TBroadcast broadcast)
    {
        if (!IsClient)
        {
            throw new InvalidOperationException($"Cannot send a broadcast before {nameof(TryStartClientDiscovery)} succeeds.");
        }

        byte[] datagram = Serialize(MessageType.Broadcast, broadcast);
        var broadcastEndPoint = new IPEndPoint(BroadcastAddress, discoveryPort);

        try
        {
            udpClient.Send(datagram, datagram.Length, broadcastEndPoint);
        }
        catch (SocketException exception)
        {
            Debug.LogException(exception);
        }
    }

    protected abstract bool ProcessBroadcast(IPEndPoint sender, TBroadcast broadcast, out TResponse response);

    protected abstract void ResponseReceived(IPEndPoint sender, TResponse response);

    private bool TryStartDiscovery(bool isServer)
    {
        StopDiscovery();

        int localPort = isServer ? discoveryPort : 0;
        UdpClient client;

        try
        {
            client = new UdpClient(localPort) { EnableBroadcast = true, MulticastLoopback = false };
        }
        catch (SocketException exception)
        {
            Debug.LogError($"Network discovery could not bind UDP port {localPort}: {exception.Message}");
            return false;
        }

        udpClient = client;
        IsServer = isServer;
        IsClient = !isServer;

        _ = ListenAsync(client);
        
        return true;
    }

    private async Task ListenAsync(UdpClient client)
    {
        while (IsActive(client))
        {
            UdpReceiveResult received;

            try
            {
                received = await client.ReceiveAsync();
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            catch (SocketException exception) when (exception.SocketErrorCode == SocketError.ConnectionReset)
            {
                // Windows reports an ICMP "port unreachable" from a previous send as a reset on the next receive.
                if (!IsActive(client)) return;

                LogUnreachablePeer();
                continue;
            }
            catch (SocketException exception)
            {
                if (!IsActive(client)) return;

                Debug.LogWarning($"Network discovery receive failed: {exception.Message}");
                await Task.Yield();
                continue;
            }

            if (!IsActive(client)) return;

            try
            {
                if (IsServer)
                {
                    await RespondToBroadcastAsync(client, received);
                }
                else
                {
                    HandleResponse(received);
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }

    private bool IsActive(UdpClient client) => udpClient == client;

    private void LogUnreachablePeer()
    {
        if (!IsClient) return;

        Debug.LogWarning($"No discovery server is listening on {BroadcastAddress}:{discoveryPort}.");
    }

    private async Task RespondToBroadcastAsync(UdpClient client, UdpReceiveResult received)
    {
        if (!TryDeserialize(received.Buffer, MessageType.Broadcast, out TBroadcast broadcast)) return;
        if (!ProcessBroadcast(received.RemoteEndPoint, broadcast, out TResponse response)) return;

        byte[] datagram = Serialize(MessageType.Response, response);
        await client.SendAsync(datagram, datagram.Length, received.RemoteEndPoint);
    }

    private void HandleResponse(UdpReceiveResult received)
    {
        if (!TryDeserialize(received.Buffer, MessageType.Response, out TResponse response)) return;

        ResponseReceived(received.RemoteEndPoint, response);
    }

    private byte[] Serialize<TPayload>(MessageType messageType, TPayload payload)
        where TPayload : INetworkSerializable, new()
    {
        using var writer = new FastBufferWriter(InitialDatagramCapacity, Allocator.Temp, MaxDatagramCapacity);

        writer.WriteValueSafe(uniqueApplicationId);
        writer.WriteByteSafe((byte)messageType);
        writer.WriteNetworkSerializable(payload);

        return writer.ToArray();
    }

    private bool TryDeserialize<TPayload>(byte[] datagram, MessageType expectedType, out TPayload payload)
        where TPayload : INetworkSerializable, new()
    {
        payload = default;

        using var reader = new FastBufferReader(datagram, Allocator.Temp);

        if (!reader.TryBeginRead(HeaderSizeInBytes)) return false;

        reader.ReadValue(out long applicationId);
        reader.ReadByte(out byte messageType);

        if (applicationId != uniqueApplicationId || messageType != (byte)expectedType) return false;

        reader.ReadNetworkSerializable(out payload);
        return true;
    }
}
