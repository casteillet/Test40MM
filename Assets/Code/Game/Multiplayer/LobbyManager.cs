using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkSingleton<LobbyManager>
{
    private readonly Dictionary<string, Player> playersById = new();

    public event Action<Dictionary<string, Player>> OnPlayerListChanged;
    public event Action<bool> OnAllPlayersReadyChanged;
    
    // TODO: Check correct execution if can access NetworkManager is not too late
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        
        NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;
    }

    // TODO: Check correct execution if can access NetworkManager is not too late
    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        
        NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvent;
    }

    public void RegisterPlayer(Player player)
    {
        var id = player.playerId.Value.ToString();
        
        playersById[id] = player;

        OnPlayerListChanged?.Invoke(playersById);
    }
    
    public void UnregisterPlayer(Player player)
    {
        var id = player.playerId.Value.ToString();

        playersById.Remove(id);
        
        OnPlayerListChanged?.Invoke(playersById);
    }
    
    private void OnConnectionEvent(NetworkManager networkManager, ConnectionEventData connectionEventData)
    {
        foreach (var playerById in playersById)
        {
            if (playerById.Value.OwnerClientId != connectionEventData.ClientId) continue;
            
            if (connectionEventData.EventType != ConnectionEvent.ClientDisconnected)
            {
                Debug.Log($"Player {playerById.Key} disconnected");
            }
            else if (connectionEventData.EventType == ConnectionEvent.ClientConnected)
            {
                Debug.Log($"Player {playerById.Key} connected");
            }
                
            return;
        }
    }

    public void AssignSpawn(string playerId, SpawnPosition spawnPosition)
    {
        if (!IsServer) return;

        if (spawnPosition != SpawnPosition.None && IsSpawnTaken(spawnPosition)) return;

        if (playersById.TryGetValue(playerId, out var player))
        {
            player.spawn.Value = spawnPosition;
        }
        
        OnAllPlayersReadyChanged?.Invoke(AllPlayersReady());
    }

    private bool IsSpawnTaken(SpawnPosition spawnPosition)
    {
        foreach (var player in playersById.Values)
        {
            if (player.spawn.Value == spawnPosition) return true;
        }
        
        return false;
    }

    private bool AllPlayersReady()
    {
        foreach (var player in playersById.Values)
        {
            if (player.spawn.Value == SpawnPosition.None) return false;
        }
        
        return true;
    }

    public void StartGame()
    {
        if (!IsServer) return;
        
        if (!AllPlayersReady()) return;

        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}