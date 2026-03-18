using System.Collections.Generic;
using TMPro;
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
    }

    // TODO: Check correct execution if can access NetworkManager is not too late
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
            var playerId = playerById.Key;
            var player = playerById.Value;

            var go = Instantiate(playerLobbyPrefab, playerListContainer);

            var text = go.GetComponentInChildren<TextMeshProUGUI>();
            var dropdown = go.GetComponentInChildren<TMP_Dropdown>();

            text.text = playerId;
            dropdown.value = (int)player.spawn.Value;
            dropdown.interactable = isServer;

            if (isServer)
            {
                dropdown.onValueChanged.AddListener(value =>
                {
                    LobbyManager.Instance.AssignSpawn(playerId, (SpawnPosition)value);
                });
            }
            else
            {
                player.spawn.OnValueChanged += (_, newValue) =>
                {
                    dropdown.SetValueWithoutNotify((int)newValue);
                };
            }
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