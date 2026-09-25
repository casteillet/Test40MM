using System.Collections.Generic;
using System.Net;
using KBCore.Refs;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class NetworkDiscoveryHud : ValidatedMonoBehaviour
{
    private const float AreaWidth = 200f;
    private const float AreaHeight = 600f;
    private const float ServerListSpacing = 40f;

    [Self, SerializeField] private NetworkDiscoveryManager networkDiscoveryManager;
    [SerializeField, FormerlySerializedAs("DrawOffset")] private Vector2 drawOffset = new(10, 210);

    private readonly Dictionary<IPEndPoint, DiscoveryResponseData> discoveredServers = new();
    private NetworkManager networkManager;

    private void OnEnable()
    {
        networkDiscoveryManager.ServerDiscovered += OnServerDiscovered;
    }

    private void OnDisable()
    {
        networkDiscoveryManager.ServerDiscovered -= OnServerDiscovered;
    }

    private void Start()
    {
        networkManager = NetworkManager.Singleton;
    }

    private void OnServerDiscovered(IPEndPoint serverEndPoint, DiscoveryResponseData response)
    {
        discoveredServers[serverEndPoint] = response;
    }

    private void OnGUI()
    {
        if (networkManager == null) return;

        GUILayout.BeginArea(new Rect(drawOffset, new Vector2(AreaWidth, AreaHeight)));

        if (networkManager.IsListening)
        {
            DrawSessionControls();
        }
        else
        {
            DrawLauncherControls();
        }

        GUILayout.EndArea();
    }

    private void DrawSessionControls()
    {
        if (networkManager.IsServer)
        {
            DrawServerDiscoveryControls();
        }

        if (GUILayout.Button("Shutdown"))
        {
            ShutdownSession();
        }
    }

    private void DrawServerDiscoveryControls()
    {
        if (networkDiscoveryManager.IsRunning)
        {
            if (GUILayout.Button("Stop Server Discovery"))
            {
                networkDiscoveryManager.StopDiscovery();
            }
        }
        else if (GUILayout.Button("Start Server Discovery"))
        {
            networkDiscoveryManager.TryStartServerDiscovery();
        }
    }

    private void DrawLauncherControls()
    {
        if (GUILayout.Button("Start Server"))
        {
            networkDiscoveryManager.StartServerNetwork();
        }

        if (GUILayout.Button("Start Host"))
        {
            networkDiscoveryManager.StartHostNetwork();
        }

        if (networkDiscoveryManager.IsClient)
        {
            DrawClientDiscoveryControls();
        }
        else if (GUILayout.Button("Discover Servers"))
        {
            discoveredServers.Clear();
            networkDiscoveryManager.StartClientNetwork();
        }
    }

    private void DrawClientDiscoveryControls()
    {
        if (GUILayout.Button("Stop Client Discovery"))
        {
            networkDiscoveryManager.StopDiscovery();
            discoveredServers.Clear();
            return;
        }

        if (GUILayout.Button("Refresh List"))
        {
            discoveredServers.Clear();
            networkDiscoveryManager.SendClientBroadcast(new DiscoveryBroadcastData());
        }

        GUILayout.Space(ServerListSpacing);

        foreach (KeyValuePair<IPEndPoint, DiscoveryResponseData> discoveredServer in discoveredServers)
        {
            if (!GUILayout.Button($"{discoveredServer.Value.ServerName} [{discoveredServer.Key.Address}]")) continue;

            networkDiscoveryManager.ConnectToServer(discoveredServer.Key, discoveredServer.Value);
            break;
        }
    }

    private void ShutdownSession()
    {
        networkManager.Shutdown();
        networkDiscoveryManager.StopDiscovery();
        discoveredServers.Clear();
    }
}
