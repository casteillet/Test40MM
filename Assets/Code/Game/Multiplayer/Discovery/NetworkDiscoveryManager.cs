using System;
using System.Net;
using System.Net.NetworkInformation;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;
using UnityUtils;

[RequireComponent(typeof(NetworkManager))]
public class NetworkDiscoveryManager : NetworkDiscovery<DiscoveryBroadcastData, DiscoveryResponseData>
{
    [Serializable] public class ServerFoundEvent : UnityEvent<IPEndPoint, DiscoveryResponseData> { };

    [SerializeField] private bool startWithServer = true;
    
    private NetworkManager networkManager;
    private bool hasStartedWithServer;
    
    public string serverName = "Server";
    public ServerFoundEvent onServerFound;
    
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
        if (startWithServer && !hasStartedWithServer && !IsRunning)
        {
            if (networkManager.IsServer)
            {
                StartServer();
                hasStartedWithServer = true;
            }
        }
    }

    protected override bool ProcessBroadcast(IPEndPoint sender, DiscoveryBroadcastData broadCast, out DiscoveryResponseData response)
    {
        response = new DiscoveryResponseData()
        {
            ServerName = serverName,
            Port = ((UnityTransport) networkManager.NetworkConfig.NetworkTransport).ConnectionData.Port,
        };
        
        return true;
    }

    protected override void ResponseReceived(IPEndPoint sender, DiscoveryResponseData response)
    {
        onServerFound.Invoke(sender, response);
    }
}