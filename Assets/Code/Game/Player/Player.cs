using System;
using KBCore.Refs;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerView))]
public class Player : NetworkBehaviour
{
    [HideInInspector] public NetworkVariable<FixedString64Bytes> playerId = new();
    
    public NetworkVariable<SpawnPosition> spawn = new();

    [field: SerializeField, Self] public PlayerView View { get; private set; }

    private void OnValidate()
    {
        this.ValidateRefs();
    }

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
            SessionManager.Instance.UnregisterPlayer(this);
        }
    }

    [Rpc(SendTo.Server)]
    private void SetPlayerIdServerRpc(string id)
    {
        playerId.Value = id;
        SessionManager.Instance.RegisterPlayer(this);
    }

    private string GetOrCreatePlayerId()
    {
        if (!PlayerPrefs.HasKey("PLAYER_ID"))
        {
            PlayerPrefs.SetString("PLAYER_ID", Guid.NewGuid().ToString());
        }

        return PlayerPrefs.GetString("PLAYER_ID");
    }
}
