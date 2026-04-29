using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityUtils;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private GameObject playerLobbyPrefab;

    private bool serverInitialized;
    private bool clientInitialized;

    private void Start()
    {
        startButton.interactable = false;

        if (NetworkManager.Singleton.IsServer)
        {
            OnServerStarted();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            OnClientStarted();
        }

        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnServerStopped += OnServerStopped;

        NetworkManager.Singleton.OnClientStarted += OnClientStarted;
        NetworkManager.Singleton.OnClientStopped += OnClientStopped;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            NetworkManager.Singleton.OnServerStopped -= OnServerStopped;

            NetworkManager.Singleton.OnClientStarted -= OnClientStarted;
            NetworkManager.Singleton.OnClientStopped -= OnClientStopped;
        }

        CleanupServer();
        CleanupClient();
    }

    private void OnServerStarted()
    {
        if (serverInitialized) return;
        
        serverInitialized = true;

        startButton.gameObject.SetActive(true);
        
        startButton.onClick.RemoveListener(OnStartClicked);
        startButton.onClick.AddListener(OnStartClicked);

        if (SessionManager.Instance)
        {
            SessionManager.Instance.networkPlayers.OnListChanged -= OnPlayerListChanged;
            SessionManager.Instance.networkPlayers.OnListChanged += OnPlayerListChanged;

            SessionManager.Instance.OnAllPlayersReadyChanged -= OnAllPlayersReadyChanged;
            SessionManager.Instance.OnAllPlayersReadyChanged += OnAllPlayersReadyChanged;

            RefreshPlayerList();
        }
    }

    private void OnServerStopped(bool _)
    {
        CleanupServer();
    }

    private void OnClientStarted()
    {
        if (clientInitialized) return;
        
        clientInitialized = true;

        startButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        if (SessionManager.Instance)
        {
            SessionManager.Instance.networkPlayers.OnListChanged -= OnPlayerListChanged;
            SessionManager.Instance.networkPlayers.OnListChanged += OnPlayerListChanged;

            RefreshPlayerList();
        }
    }

    private void OnClientStopped(bool _)
    {
        CleanupClient();
    }

    private void CleanupServer()
    {
        if (!serverInitialized) return;
        
        serverInitialized = false;

        startButton.onClick.RemoveListener(OnStartClicked);

        if (SessionManager.Instance)
        {
            SessionManager.Instance.networkPlayers.OnListChanged -= OnPlayerListChanged;
            SessionManager.Instance.OnAllPlayersReadyChanged -= OnAllPlayersReadyChanged;
        }
    }

    private void CleanupClient()
    {
        if (!clientInitialized) return;
        
        clientInitialized = false;

        if (SessionManager.Instance)
        {
            SessionManager.Instance.networkPlayers.OnListChanged -= OnPlayerListChanged;
        }
    }

    private void OnPlayerListChanged(NetworkListEvent<PlayerLobbyState> _)
    {
        RefreshPlayerList();
    }

    private void RefreshPlayerList()
    {
        if (!SessionManager.Instance) return;

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