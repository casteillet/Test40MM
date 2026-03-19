using System;
using System.Net;
using System.Net.NetworkInformation;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;
using UnityUtils;
using VInspector;

[RequireComponent(typeof(NetworkManager))]
public class NetworkDiscoveryManager : NetworkDiscovery<DiscoveryBroadcastData, DiscoveryResponseData>
{
    [Serializable] public class ServerFoundEvent : UnityEvent<IPEndPoint, DiscoveryResponseData> { };

    [SerializeField] private bool startWithServer = true;
    
#if UNITY_EDITOR
    private enum ConnectionTestType { Wireless, Local }
    
    [Header("Debug")]
    [SerializeField] private bool localTestMode;
    [ShowIf("localTestMode"), SerializeField] private ConnectionTestType connectionTestType;[EndIf]
#endif
    
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
        if (localTestMode)
        {
            if (connectionTestType == ConnectionTestType.Wireless)
            {
                ipAddress = NetworkHelper.GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            }
            else if (connectionTestType == ConnectionTestType.Local)
            {
                return;
            }
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