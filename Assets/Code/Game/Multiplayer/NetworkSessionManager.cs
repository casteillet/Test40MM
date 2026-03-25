using Unity.Netcode;
using UnityEngine;
using VInspector;

public class NetworkSessionManager : MonoBehaviour
{
    [SerializeField] private NetworkDiscoveryManager networkDiscoveryManager;
    
    public void StartServer()
    {
        if (!NetworkManager.Singleton.StartServer()) return;
        
        networkDiscoveryManager.StartServer();
        
        NetworkSceneManager.Instance.LoadLobbyAsServer();
    }
    
    public void StartClient()
    {
        networkDiscoveryManager.StartClient();
        networkDiscoveryManager.ClientBroadcast(new DiscoveryBroadcastData());
    }

    [Button]
    public void Shutdown()
    {
        NetworkManager.Singleton.Shutdown();
    }
}