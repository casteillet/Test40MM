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
    [SerializeField] private ExampleNetworkDiscovery networkDiscovery;
    
    private NetworkManager networkManager;
    private Dictionary<IPAddress, DiscoveryResponseData> discoveredServers = new();

    public Vector2 DrawOffset = new Vector2(10, 210);
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (networkDiscovery) return;
        
        UnityEventTools.AddPersistentListener(networkDiscovery.OnServerFound, OnServerFound);
        Undo.RecordObjects(new Object[] { this, networkDiscovery}, "Set NetworkDiscovery");
    }
#endif
    
    private void Awake()
    {
        networkDiscovery = GetComponent<ExampleNetworkDiscovery>();
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
                networkDiscovery.StopDiscovery();
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
        
        if (networkDiscovery.IsRunning)
        {
            if (GUILayout.Button("Stop Client Discovery"))
            {
                networkDiscovery.StopDiscovery();
                discoveredServers.Clear();
            }
            
            if (GUILayout.Button("Refresh List"))
            {
                discoveredServers.Clear();
                networkDiscovery.ClientBroadcast(new DiscoveryBroadcastData());
            }
            
            GUILayout.Space(40);
            
            foreach (var discoveredServer in discoveredServers)
            {
                if (GUILayout.Button($"{discoveredServer.Value.ServerName}[{discoveredServer.Key}]"))
                {
                    var transport = (UnityTransport)networkManager.NetworkConfig.NetworkTransport;
                    transport.SetConnectionData(discoveredServer.Key.ToString(), discoveredServer.Value.Port);
                    networkManager.StartClient();
                }
            }
        }
        else
        {
            if (GUILayout.Button("Discover Servers"))
            {
                networkDiscovery.StartClient();
                networkDiscovery.ClientBroadcast(new DiscoveryBroadcastData());
            }
        }
    }

    private void ServerControlsGUI()
    {
        if (networkDiscovery.IsRunning)
        {
            if (GUILayout.Button("Stop Server Discovery"))
            {
                networkDiscovery.StopDiscovery();
            }
        }
        else
        {
            if (GUILayout.Button("Start Server Discovery"))
            {
                networkDiscovery.StartServer();
            }
        }
    }
}
