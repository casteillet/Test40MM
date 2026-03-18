using System;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityUtils;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Transform playerListContainer;
    [SerializeField] private GameObject playerLobbyPrefab;
    
    private readonly bool isServer = NetworkManager.Singleton.IsServer;
    
    private void Start()
    {
        
        startButton.gameObject.SetActive(!isServer);
        
        LobbyManager.Instance.OnPlayerListChanged += OnPlayerListChanged;
        
        if (!isServer) return;
        
        LobbyManager.Instance.OnPlayerReady += OnPlayerReady;
        
        startButton.onClick.AddListener(OnStartClicked);
    }

    // TODO: Check correct execution if can access NetworkManager is not too late
    private void OnDestroy()
    {
        LobbyManager.Instance.OnPlayerListChanged -= OnPlayerListChanged;
        
        if (!isServer) return;
        
        LobbyManager.Instance.OnPlayerReady -= OnPlayerReady;
        
        startButton.onClick.RemoveListener(OnStartClicked);
    }
    
    private void OnPlayerListChanged(Dictionary<string, Player> players)
    {
        RefreshPlayerList(players);
    }

    private void RefreshPlayerList(Dictionary<string, Player> players)
    {
        playerListContainer.DestroyChildren();
        
        foreach (var player in players)
        {
            var goInstance = Instantiate(playerLobbyPrefab, playerListContainer);
            goInstance.GetComponent<TextMeshProUGUI>().text = player.Key;
            goInstance.GetComponent<TMP_Dropdown>().value = (int)player.Value.spawn.Value;
            goInstance.GetComponent<TMP_Dropdown>().enabled = isServer;

            if (isServer)
            {
                goInstance.GetComponent<TMP_Dropdown>().onValueChanged.AddListener(OnDropdownValueChanged);
            }
        }
    }

    private void OnDropdownValueChanged(int value)
    {
        // TODO : AssignSpawn to player
        
        RefreshPlayerList();
    }

    private void OnPlayerReady(bool playerReady)
    {
        startButton.enabled = playerReady;
    }

    private void OnStartClicked()
    {
        LobbyManager.Instance.StartGame();
    }
}