using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityUtils;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private GameObject playerLobbyPrefab;
    
    private void Start()
    {
        startButton.interactable = false;
        
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnServerStopped += OnServerStopped;
        
        NetworkManager.Singleton.OnClientStarted += OnClientStarted;
        NetworkManager.Singleton.OnClientStopped += OnClientStopped;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            startButton.onClick.RemoveListener(OnStartClicked);

            if (LobbyManager.Instance)
            {
                LobbyManager.Instance.OnAllPlayersReadyChanged -= OnAllPlayersReadyChanged;
            }
        }
            
        NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
        
        NetworkManager.Singleton.OnClientStarted -= OnClientStarted;
        NetworkManager.Singleton.OnClientStopped -= OnClientStopped;
        
        if (LobbyManager.Instance)
        {
            LobbyManager.Instance.networkPlayers.OnListChanged -= OnPlayerListChanged;
        }
    }

    private void OnServerStarted()
    {
        startButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        startButton.onClick.AddListener(OnStartClicked);
        
        LobbyManager.Instance.networkPlayers.OnListChanged += OnPlayerListChanged;
        LobbyManager.Instance.OnAllPlayersReadyChanged += OnAllPlayersReadyChanged;
    }
    
    private void OnServerStopped(bool safe)
    {
        startButton.onClick.RemoveListener(OnStartClicked);
        
        if (!LobbyManager.Instance) return;
        
        LobbyManager.Instance.networkPlayers.OnListChanged -= OnPlayerListChanged;
        LobbyManager.Instance.OnAllPlayersReadyChanged -= OnAllPlayersReadyChanged;
    }

    private void OnClientStarted()
    {
        startButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        
        LobbyManager.Instance.networkPlayers.OnListChanged += OnPlayerListChanged;
        
        RefreshPlayerList();
    }

    private void OnClientStopped(bool safe)
    { 
        if (!LobbyManager.Instance) return;
        
        LobbyManager.Instance.networkPlayers.OnListChanged -= OnPlayerListChanged;
    }
    
    private void OnPlayerListChanged(NetworkListEvent<PlayerLobbyState> _)
    {
        RefreshPlayerList();
    }
    
    private void RefreshPlayerList()
    {
        playerListContainer.DestroyChildren();

        foreach (var playerState in LobbyManager.Instance.networkPlayers)
        {
            var go = Instantiate(playerLobbyPrefab, playerListContainer);

            var item = go.GetComponent<LobbyPlayerUI>();
            item.Initialize(playerState, NetworkManager.Singleton.IsServer);
        }
    }

    private void OnAllPlayersReadyChanged(bool playersReady)
    {
        startButton.interactable = playersReady;
    }

    private void OnStartClicked()
    {
        LobbyManager.Instance.StartGame();
    }
}