using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SessionManager : NetworkPersistentSingleton<SessionManager>
{
    private readonly Dictionary<string, PlayerData> playerDataById = new();
    private readonly Dictionary<ulong, Player> playersByClientId = new();
    
    private NetworkTransport networkTransport;
    
    public NetworkList<PlayerLobbyState> networkPlayers = new();

    public event Action<bool> OnAllPlayersReadyChanged;

    public event Action<Player> OnPlayerRegistered;
    public IEnumerable<Player> Players => playersByClientId.Values;

    public bool TryGetPlayer(ulong clientId, out Player player) => playersByClientId.TryGetValue(clientId, out player);

    public void RegisterPlayer(Player player)
    {
        if (!IsServer || !IsSpawned) return;

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
                ClientId = clientId,
                PlayerId = id,
                Spawn = SpawnPosition.None
            };

            playerDataById.Add(id, newData);
        }

        networkPlayers.Add(new PlayerLobbyState
        {
            PlayerId = id,
            ClientId = clientId,
            Spawn = player.spawn.Value,
        });

        Debug.Log($"Player {id} connected");

        OnPlayerRegistered?.Invoke(player);
        OnAllPlayersReadyChanged?.Invoke(AllPlayersReady());
    }

    public void UnregisterPlayer(Player player)
    {
        if (!IsServer) return;

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

    public void AssignSpawn(string playerId, SpawnPosition spawnPosition)
    {
        if (!IsServer) return;

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
            if (networkPlayers[i].PlayerId.Value == playerId)
            {
                var player = networkPlayers[i];
                player.Spawn = spawnPosition;
                networkPlayers[i] = player;
                break;
            }
        }

        OnAllPlayersReadyChanged?.Invoke(AllPlayersReady());
    }

    public bool AllPlayersReady()
    {
        foreach (var player in playersByClientId.Values)
        {
            if (player.spawn.Value == SpawnPosition.None) return false;

            foreach (var other in playersByClientId.Values)
            {
                if (other == player) continue;

                if (player.spawn.Value == other.spawn.Value) return false;
            }
        }

        return true;
    }

    private void OnApplicationQuit()
    {
        ShutdownNetwork();
    }
    
    private void ShutdownNetwork()
    {
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}
