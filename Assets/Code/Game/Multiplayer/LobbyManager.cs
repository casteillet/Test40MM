using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkSingleton<LobbyManager>
{
    private readonly Dictionary<string, Player> playersById = new();

    public event Action<Dictionary<string, Player>> OnPlayerListChanged;
    public event Action<bool> OnAllPlayersReadyChanged;
    
    public void RegisterPlayer(Player player)
    {
        var id = player.playerId.Value.ToString();
        
        if (playersById.TryGetValue(id, out var existingPlayer))
        {
            playersById[id] = player;

            // TODO: Reassign player values, by calling a load system to the local player
            player.spawn.Value = existingPlayer.spawn.Value;
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
    
    public Dictionary<string, Player> GetPlayers()
    {
        return playersById;
    }
}