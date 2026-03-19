using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkSingleton<LobbyManager>
{
    private readonly Dictionary<string, PlayerData> playerDataById = new();
    private readonly Dictionary<ulong, Player> playersByClientId = new();
    
    public NetworkList<PlayerLobbyState> networkPlayers;
    public event Action<bool> OnAllPlayersReadyChanged;

    protected override void Awake()
    {
        base.Awake();
        
        networkPlayers = new NetworkList<PlayerLobbyState>();
    }
    
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
        
        networkPlayers.Add(new PlayerLobbyState
        {
            PlayerId = id,
            ClientId = clientId,
            Spawn = SpawnPosition.None
        });

        Debug.Log($"Player {id} connected");

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
        
        for (var i = 0; i < networkPlayers.Count; i++)
        {
            if (networkPlayers[i].PlayerId.ToString() == id)
            {
                networkPlayers.RemoveAt(i);
                break;
            }
        }

        Debug.Log($"Player {id} disconnected");

        OnAllPlayersReadyChanged?.Invoke(AllPlayersReady());
    }
    
    public bool TryAssignSpawn(string playerId, SpawnPosition spawnPosition)
    {
        if (!IsServer) return false;

        if (spawnPosition != SpawnPosition.None && IsSpawnTaken(spawnPosition)) return false;

        if (playerDataById.TryGetValue(playerId, out var data))
        {
            data.Spawn = spawnPosition;

            if (playersByClientId.TryGetValue(data.ClientId, out var player))
            {
                player.spawn.Value = spawnPosition;
            }
        }
        
        for (var i = 0; i < networkPlayers.Count; i++)
        {
            if (networkPlayers[i].PlayerId.Equals(playerId))
            {
                var player = networkPlayers[i];
                player.Spawn = spawnPosition;
                networkPlayers[i] = player;
                break;
            }
        }

        networkPlayers.IsDirty();
        OnAllPlayersReadyChanged?.Invoke(AllPlayersReady());
        return true;
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
}