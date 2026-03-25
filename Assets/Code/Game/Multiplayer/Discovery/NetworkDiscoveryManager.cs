using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;
using UnityUtils;
using VInspector;

public class NetworkDiscoveryManager : NetworkDiscovery<DiscoveryBroadcastData, DiscoveryResponseData>
{
    [Serializable] public class ServerFoundEvent : UnityEvent<IPEndPoint, DiscoveryResponseData> { };
    
#if UNITY_EDITOR
    private enum ConnectionTestType { Wireless, Local }
    
    [Header("Debug")]
    [SerializeField] private bool localTestMode;
    [ShowIf("localTestMode"), SerializeField] private ConnectionTestType connectionTestType;[EndIf]
#endif
    
    private Dictionary<IPAddress, DiscoveryResponseData> discoveredServers = new();
    
    public string serverName = "Server";
    
    
    private void Start()
    {
        var unityTransport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;

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
        
        if (ipAddress.IsNullOrEmpty()) return;
        
        unityTransport.SetConnectionData(ipAddress, Port);
    }

    protected override bool ProcessBroadcast(IPEndPoint sender, DiscoveryBroadcastData broadCast, out DiscoveryResponseData response)
    {
        response = new DiscoveryResponseData()
        {
            ServerName = serverName,
            Port = ((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport).ConnectionData.Port,
        };
        
        return true;
    }

    protected override void ResponseReceived(IPEndPoint sender, DiscoveryResponseData response)
    {
        OnServerFound(sender, response);
    }
    
    private void OnServerFound(IPEndPoint sender, DiscoveryResponseData response)
    {
        discoveredServers[sender.Address] = response;
        Debug.Log($"Server {sender.Address}:{sender.Port} has been found");
        TryConnectToDiscoveredServer();
    }

    public void TryConnectToDiscoveredServer()
    {
        var discoveredServer = discoveredServers.First();
        
        var transport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (localTestMode)
        {
            if (connectionTestType == ConnectionTestType.Local)
            {
                var address = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
                transport.SetConnectionData(address.ConnectionData.Address, discoveredServer.Value.Port);
            }
        }
#else
        transport.SetConnectionData(discoveredServer.Key.ToString(), discoveredServer.Value.Port);
#endif   
        Debug.Log($"a Address: {transport.ConnectionData.Address}, Port: {transport.ConnectionData.Port}");
        Debug.Log($"b Address: {discoveredServer.Key}, Port: {discoveredServer.Value.Port}");

        NetworkManager.Singleton.StartClient();
        Debug.Log("NEED TO STOP DISCOVERY BUT CRASH");
        //StopDiscovery();
//         if (NetworkManager.Singleton.StartClient())
//         {
//             StopDiscovery(); 
//         }
    }
}