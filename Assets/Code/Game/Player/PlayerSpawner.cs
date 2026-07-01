using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    public Transform frontSpawn;
    public Transform backSpawn;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        Debug.Log("Placing players"); // TODO: place only the newly-connected client if joining mid-game

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObj = client.PlayerObject;
            if (playerObj == null) continue;                                // supervisor host has no player object
            if (!playerObj.TryGetComponent<Player>(out var player)) continue;

            var spawn = player.spawn.Value == SpawnPosition.Front
                ? frontSpawn
                : backSpawn;

            playerObj.transform.position = spawn.position;
        }
    }
}
