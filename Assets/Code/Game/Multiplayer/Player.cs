using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [HideInInspector] public NetworkVariable<FixedString64Bytes> playerId = new();
    public NetworkVariable<SpawnPosition> spawn = new();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            var id = GetOrCreatePlayerId();
            SetPlayerIdServerRpc(id);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            LobbyManager.Instance.UnregisterPlayer(this);
        }
    }

    [Rpc(SendTo.Server, DeferLocal = true)]
    private void SetPlayerIdServerRpc(string id)
    {
        // playerId.Value = id;
        playerId.Value = System.Guid.NewGuid().ToString();
        
        LobbyManager.Instance.RegisterPlayer(this);
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