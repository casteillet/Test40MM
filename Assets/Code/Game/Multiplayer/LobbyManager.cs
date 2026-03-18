using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkSingleton<LobbyManager>
{
    private readonly Dictionary<string, Player> playersById = new();

    public event Action<Dictionary<string, Player>> OnPlayerListChanged;
    public event Action<bool> OnPlayerReady;
    
    // TODO: Check correct execution if can access NetworkManager is not too late
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        
        // TODO: Replace by OnConnectionEvent instead
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }
    
    // TODO: Check correct execution if can access NetworkManager is not too late
    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        
        // TODO: Replace by OnConnectionEvent instead
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    public void RegisterPlayer(Player player)
    {
        var id = player.playerId.Value.ToString();

        if (playersById.ContainsKey(id))
        {
            playersById[id] = player;
        }
        else
        {
            playersById.Add(id, player);
        }

        OnPlayerListChanged?.Invoke(playersById);
    }
    
    public void UnregisterPlayer(Player player)
    {
        var id = player.playerId.Value.ToString();

        playersById.Remove(id);
        
        OnPlayerListChanged?.Invoke(playersById);
    }

    private void OnClientDisconnected(ulong clientId)
    {
        foreach (var playerById in playersById)
        {
            if (playerById.Value.OwnerClientId == clientId)
            {
                Debug.Log($"Player {playerById.Key} disconnected");
                return;
            }
        }
    }

    private bool IsSpawnTaken(SpawnPosition spawnPosition)
    {
        foreach (var player in playersById.Values)
        {
            if (player.spawn.Value == spawnPosition) return true;
        }
        
        return false;
    }

    public void AssignSpawn(string playerId, SpawnPosition spawnPosition)
    {
        if (!IsServer) return;

        if (IsSpawnTaken(spawnPosition)) return;

        if (playersById.TryGetValue(playerId, out var player))
        {
            player.spawn.Value = spawnPosition;
        }
        
        OnPlayerReady?.Invoke(AllPlayersReady());
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

        NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}