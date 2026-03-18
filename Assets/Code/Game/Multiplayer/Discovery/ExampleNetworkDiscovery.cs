using System;
using System.Net;
using System.Net.NetworkInformation;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;
using UnityUtils;

[RequireComponent(typeof(NetworkManager))]
public class ExampleNetworkDiscovery : NetworkDiscovery<DiscoveryBroadcastData, DiscoveryResponseData>
{
    [Serializable] public class ServerFoundEvent : UnityEvent<IPEndPoint, DiscoveryResponseData> { };

    private NetworkManager networkManager;
    
    [SerializeField]
    [Tooltip("If true NetworkDiscovery will make the server visible and answer to client broadcasts as soon as netcode starts running as server.")]
    bool m_StartWithServer = true;

    public string ServerName = "EnterName";

    public ServerFoundEvent OnServerFound;
    
    private bool m_HasStartedWithServer;

    private void Start()
    {
        networkManager = NetworkManager.Singleton;
        
        var unityTransport = (UnityTransport)networkManager.NetworkConfig.NetworkTransport;

        var ipAddress = NetworkHelper.GetLocalIPv4(NetworkInterfaceType.Ethernet);
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (ipAddress.IsNullOrEmpty())
        {
            ipAddress = NetworkHelper.GetLocalIPv4(NetworkInterfaceType.Wireless80211);
        }
#endif
        
        unityTransport.SetConnectionData(ipAddress, Port);
    }

    public void Update()
    {
        if (m_StartWithServer && !m_HasStartedWithServer && !IsRunning)
        {
            if (networkManager.IsServer)
            {
                StartServer();
                m_HasStartedWithServer = true;
            }
        }
    }

    protected override bool ProcessBroadcast(IPEndPoint sender, DiscoveryBroadcastData broadCast, out DiscoveryResponseData response)
    {
        response = new DiscoveryResponseData()
        {
            ServerName = ServerName,
            Port = ((UnityTransport) networkManager.NetworkConfig.NetworkTransport).ConnectionData.Port,
        };
        return true;
    }

    protected override void ResponseReceived(IPEndPoint sender, DiscoveryResponseData response)
    {
        OnServerFound.Invoke(sender, response);
    }
}