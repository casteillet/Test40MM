using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

public class ExampleNetworkDiscoveryHud : MonoBehaviour
{
    [SerializeField] private NetworkDiscoveryManager networkDiscoveryManager;
    
    private NetworkManager networkManager;
    private Dictionary<IPAddress, DiscoveryResponseData> discoveredServers = new();

    public Vector2 DrawOffset = new Vector2(10, 210);
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (networkDiscoveryManager) return;
        
        UnityEventTools.AddPersistentListener(networkDiscoveryManager.onServerFound, OnServerFound);
        Undo.RecordObjects(new Object[] { this, networkDiscoveryManager}, "Set NetworkDiscovery");
    }
#endif
    
    private void Awake()
    {
        networkDiscoveryManager = GetComponent<NetworkDiscoveryManager>();
    }

    private void Start()
    {
        networkManager = NetworkManager.Singleton;
    }

    private void OnServerFound(IPEndPoint sender, DiscoveryResponseData response)
    {
        discoveredServers[sender.Address] = response;
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(DrawOffset, new Vector2(200, 600)));

        if (networkManager.IsServer || networkManager.IsClient)
        {
            // if (networkManager.IsServer)
            // {
            //     ServerControlsGUI();
            // }
            
            if (GUILayout.Button("Shutdown"))
            {
                networkManager.Shutdown();
                networkDiscoveryManager.StopDiscovery();
            }
        }
        else
        {
            ClientSearchGUI();
        }

        GUILayout.EndArea();
    }

    private void ClientSearchGUI()
    {
        if (GUILayout.Button("Start Server"))
        {
            networkManager.StartServer();
        }
        
        if (GUILayout.Button("Start Host"))
        {
            networkManager.StartHost();
        }
        
        if (networkDiscoveryManager.IsRunning)
        {
            if (GUILayout.Button("Stop Client Discovery"))
            {
                networkDiscoveryManager.StopDiscovery();
                discoveredServers.Clear();
            }
            
            if (GUILayout.Button("Refresh List"))
            {
                discoveredServers.Clear();
                networkDiscoveryManager.ClientBroadcast(new DiscoveryBroadcastData());
            }
            
            GUILayout.Space(40);
            
            foreach (var discoveredServer in discoveredServers)
            {
                if (GUILayout.Button($"{discoveredServer.Value.ServerName}[{discoveredServer.Key}]"))
                {
                    var transport = (UnityTransport)networkManager.NetworkConfig.NetworkTransport;
                    transport.SetConnectionData(discoveredServer.Key.ToString(), discoveredServer.Value.Port);
                    Debug.Log($"Address: {discoveredServer.Key}, Port: {discoveredServer.Value.Port}");
                    networkManager.StartClient();
                }
            }
        }
        else
        {
            if (GUILayout.Button("Discover Servers"))
            {
                networkDiscoveryManager.StartClient();
                networkDiscoveryManager.ClientBroadcast(new DiscoveryBroadcastData());
            }
        }
    }

    private void ServerControlsGUI()
    {
        if (networkDiscoveryManager.IsRunning)
        {
            if (GUILayout.Button("Stop Server Discovery"))
            {
                networkDiscoveryManager.StopDiscovery();
            }
        }
        else
        {
            if (GUILayout.Button("Start Server Discovery"))
            {
                networkDiscoveryManager.StartServer();
            }
        }
    }
}
