using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkSingleton<LobbyManager>
{
    private readonly Dictionary<string, PlayerData> playerDataById = new();
    private readonly Dictionary<ulong, Player> playersByClientId = new();

    public event Action<Dictionary<string, Player>> OnPlayerListChanged;
    public event Action<bool> OnAllPlayersReadyChanged;
    
    public void RegisterPlayer(Player player)
    {
        var id = player.playerId.Value.ToString();
        var clientId = player.OwnerClientId;
        
        playersByClientId[clientId] = player;
        
        if (playerDataById.TryGetValue(id, out var data)) // Rejoin existing session
        {
            data.ClientId = clientId;

            player.spawn.Value = data.Spawn;
        }
        else // New player
        {
            var newData = new PlayerData
            {
                PlayerId = id,
                Spawn = SpawnPosition.None,
                ClientId = clientId
            };

            playerDataById.Add(id, newData);
        }

        Debug.Log($"Player {id} connected");

        OnPlayerListChanged?.Invoke(GetPlayers());
        OnAllPlayersReadyChanged?.Invoke(AllPlayersReady());
    }
    
    public void UnregisterPlayer(Player player)
    {
        var id = player.playerId.Value.ToString();
        
        if (playerDataById.TryGetValue(id, out var data))
        {
            playersByClientId.Remove(data.ClientId);
            data.ClientId = 0;
        }

        Debug.Log($"Player {id} disconnected");

        OnPlayerListChanged?.Invoke(GetPlayers());
        OnAllPlayersReadyChanged?.Invoke(AllPlayersReady());
    }
    
    public void AssignSpawn(string playerId, SpawnPosition spawnPosition)
    {
        if (!IsServer) return;

        if (spawnPosition != SpawnPosition.None && IsSpawnTaken(spawnPosition)) return;

        if (playerDataById.TryGetValue(playerId, out var data))
        {
            data.Spawn = spawnPosition;

            if (playersByClientId.TryGetValue(data.ClientId, out var player))
            {
                player.spawn.Value = spawnPosition;
            }
        }

        // OnPlayerListChanged?.Invoke(GetPlayers());
        OnAllPlayersReadyChanged?.Invoke(AllPlayersReady());
    }

    private bool IsSpawnTaken(SpawnPosition spawnPosition)
    {
        foreach (var player in playersByClientId.Values)
        {
            if (player.spawn.Value == spawnPosition) return true;
        }
        
        return false;
    }

    private bool AllPlayersReady()
    {
        foreach (var player in playersByClientId.Values)
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
    
    public Dictionary<string, Player> GetPlayers()
    {
        var result = new Dictionary<string, Player>();

        foreach (var data in playerDataById.Values)
        {
            if (playersByClientId.TryGetValue(data.ClientId, out var player))
            {
                result[data.PlayerId] = player;
            }
        }

        return result;
    }
}