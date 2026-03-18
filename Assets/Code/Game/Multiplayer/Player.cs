using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public NetworkVariable<FixedString128Bytes> playerId = new();
    public NetworkVariable<SpawnPosition> spawn = new();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            var id = GetOrCreatePlayerId();
            SetPlayerIdServerRpc(id);
        }

        if (IsServer)
        {
            LobbyManager.Instance.RegisterPlayer(this);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            LobbyManager.Instance.UnregisterPlayer(this);
        }
    }

    [ServerRpc]
    private void SetPlayerIdServerRpc(string id)
    {
        playerId.Value = id;
    }

    private string GetOrCreatePlayerId()
    {
        if (!PlayerPrefs.HasKey("PLAYER_ID"))
        {
            var id = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("PLAYER_ID", id);
        }

        return PlayerPrefs.GetString("PLAYER_ID");
    }
}