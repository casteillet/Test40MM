using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityUtils;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private GameObject playerLobbyPrefab;
    
    private bool isServer;

    private void Start()
    {
        isServer = NetworkManager.Singleton.IsServer;
        
        startButton.gameObject.SetActive(isServer);
        
        LobbyManager.Instance.OnPlayerListChanged += OnPlayerListChanged;

        if (isServer)
        {
            LobbyManager.Instance.OnAllPlayersReadyChanged += OnAllPlayersReadyChanged;
            
            startButton.onClick.AddListener(OnStartClicked);
        }
        else
        {
            OnPlayerListChanged(LobbyManager.Instance.GetPlayers());
        }
    }

    private void OnDestroy()
    {
        if (!LobbyManager.Instance) return;
        
        LobbyManager.Instance.OnPlayerListChanged -= OnPlayerListChanged;

        if (isServer)
        {
            LobbyManager.Instance.OnAllPlayersReadyChanged -= OnAllPlayersReadyChanged;

            startButton.onClick.RemoveListener(OnStartClicked);
        }
    }
    
    private void OnPlayerListChanged(Dictionary<string, Player> players)
    {
        RefreshPlayerList(players);
    }
    
    private void RefreshPlayerList(Dictionary<string, Player> playersById)
    {
        playerListContainer.DestroyChildren();

        foreach (var playerById in playersById)
        {
            var go = Instantiate(playerLobbyPrefab, playerListContainer);

            var item = go.GetComponent<LobbyPlayerUI>();
            item.Initialize(playerById.Value, playerById.Key, isServer);
        }
    }

    private void OnAllPlayersReadyChanged(bool playersReady)
    {
        startButton.enabled = playersReady;
    }

    private void OnStartClicked()
    {
        LobbyManager.Instance.StartGame();
    }
}