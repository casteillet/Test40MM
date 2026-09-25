using System;
using System.Net;
using System.Net.NetworkInformation;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityUtils;
using VInspector;

public class NetworkDiscoveryManager : NetworkDiscovery<DiscoveryBroadcastData, DiscoveryResponseData>
{
    private enum ConnectionTestType { Wireless, Local }

    private const string ListenOnAllInterfacesAddress = "0.0.0.0";

    [Header("Debug")]
    [SerializeField] private bool localTestMode;
    [ShowIf(nameof(localTestMode))]
    [SerializeField] private ConnectionTestType connectionTestType;
    [EndIf]

    [Header("Session")]
    [SerializeField] private string serverName = "Server";
    [SerializeField] private bool autoConnectToFirstServer = true;

    public event Action<IPEndPoint, DiscoveryResponseData> ServerDiscovered;

    private static UnityTransport Transport => (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;

    private bool IsLocalTestModeEnabled
    {
        get
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            return localTestMode;
#else
            return false;
#endif
        }
    }

    private bool UsesLoopbackTest => IsLocalTestModeEnabled && connectionTestType == ConnectionTestType.Local;

    private bool UsesWirelessTest => IsLocalTestModeEnabled && connectionTestType == ConnectionTestType.Wireless;

    protected override IPAddress BroadcastAddress => UsesLoopbackTest ? IPAddress.Loopback : base.BroadcastAddress;

    private void Start()
    {
        ConfigureTransportAddress();
    }

    public void StartServerNetwork()
    {
        StartSession(NetworkManager.Singleton.StartServer);
    }

    public void StartHostNetwork()
    {
        StartSession(NetworkManager.Singleton.StartHost);
    }

    public void StartClientNetwork()
    {
        if (!TryStartClientDiscovery()) return;

        SendClientBroadcast(new DiscoveryBroadcastData());
    }

    public void ConnectToServer(IPEndPoint serverEndPoint, DiscoveryResponseData response)
    {
        if (NetworkManager.Singleton.IsListening) return;

        string serverAddress = UsesLoopbackTest ? Transport.ConnectionData.Address : serverEndPoint.Address.ToString();
        Transport.SetConnectionData(serverAddress, response.Port);

        if (NetworkManager.Singleton.StartClient())
        {
            StopDiscovery();
        }
        else
        {
            Debug.LogError($"Failed to connect to server '{response.ServerName}' at {serverAddress}:{response.Port}.");
        }
    }

    protected override bool ProcessBroadcast(IPEndPoint sender, DiscoveryBroadcastData broadcast, out DiscoveryResponseData response)
    {
        response = new DiscoveryResponseData
        {
            ServerName = serverName,
            Port = Transport.ConnectionData.Port,
        };

        return true;
    }

    protected override void ResponseReceived(IPEndPoint sender, DiscoveryResponseData response)
    {
        ServerDiscovered?.Invoke(sender, response);

        if (autoConnectToFirstServer)
        {
            ConnectToServer(sender, response);
        }
    }

    private void StartSession(Func<bool> startNetworkManager)
    {
        if (!HasDistinctPorts()) return;
        if (!TryStartServerDiscovery()) return;

        if (!startNetworkManager())
        {
            StopDiscovery();
            return;
        }

        NetworkSceneManager.Instance.LoadLobbyAsServer();
    }

    private void ConfigureTransportAddress()
    {
        if (UsesLoopbackTest) return;

        NetworkInterfaceType interfaceType = UsesWirelessTest ? NetworkInterfaceType.Wireless80211 : NetworkInterfaceType.Ethernet;
        string localAddress = NetworkHelper.GetLocalIPv4(interfaceType);

        if (localAddress.IsNullOrEmpty()) return;

        Transport.SetConnectionData(localAddress, Transport.ConnectionData.Port, ListenOnAllInterfacesAddress);
    }

    private bool HasDistinctPorts()
    {
        if (Transport.ConnectionData.Port != DiscoveryPort) return true;

        Debug.LogError($"{nameof(UnityTransport)} and {nameof(NetworkDiscoveryManager)} both use port {DiscoveryPort}. Give them different ports.");
        return false;
    }
}
