using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkSceneManager : PersistentSingleton<NetworkSceneManager>
{
    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.OnServerStopped += OnServerStopped;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
            NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId) return;
        
        LoadMainMenu();
    }

    private void OnServerStopped(bool _)
    {
        LoadMainMenu();
    }

    private void LoadMainMenu() => LoadScene("MainMenu");
    public void LoadLobbyAsServer() => LoadNetworkScene("Lobby");
    public void LoadGameAsServer() => LoadNetworkScene("Game");

    private void LoadNetworkScene(string sceneName)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    
    private void LoadScene(string sceneName)
    {
        if (NetworkManager.Singleton && NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }
        
        SceneManager.LoadScene(sceneName);
    }
}