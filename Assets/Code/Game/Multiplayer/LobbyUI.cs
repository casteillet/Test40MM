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
        if (NetworkManager.Singleton)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                startButton.onClick.RemoveListener(OnStartClicked);

                if (SessionManager.Instance)
                {
                    SessionManager.Instance.OnAllPlayersReadyChanged -= OnAllPlayersReadyChanged;
                }
            }

            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            NetworkManager.Singleton.OnServerStopped -= OnServerStopped;

            NetworkManager.Singleton.OnClientStarted -= OnClientStarted;
            NetworkManager.Singleton.OnClientStopped -= OnClientStopped;
        }

        if (SessionManager.Instance && SessionManager.Instance.networkPlayers != null)
        {
            SessionManager.Instance.networkPlayers.OnListChanged -= OnPlayerListChanged;
        }
    }

    private void OnServerStarted()
    {
        startButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        startButton.onClick.AddListener(OnStartClicked);
        
        SessionManager.Instance.networkPlayers.OnListChanged += OnPlayerListChanged;
        SessionManager.Instance.OnAllPlayersReadyChanged += OnAllPlayersReadyChanged;
    }
    
    private void OnServerStopped(bool safe)
    {
        startButton.onClick.RemoveListener(OnStartClicked);
        
        if (!SessionManager.Instance) return;
        
        SessionManager.Instance.networkPlayers.OnListChanged -= OnPlayerListChanged;
        SessionManager.Instance.OnAllPlayersReadyChanged -= OnAllPlayersReadyChanged;
    }

    private void OnClientStarted()
    {
        startButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        
        SessionManager.Instance.networkPlayers.OnListChanged += OnPlayerListChanged;
    }

    private void OnClientStopped(bool safe)
    { 
        if (!SessionManager.Instance) return;
        
        SessionManager.Instance.networkPlayers.OnListChanged -= OnPlayerListChanged;
    }
    
    private void OnPlayerListChanged(NetworkListEvent<PlayerLobbyState> _)
    {
        RefreshPlayerList();
    }

    private void RefreshPlayerList()
    {
        playerListContainer.DestroyChildren();
        
        foreach (var playerState in SessionManager.Instance.networkPlayers)
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