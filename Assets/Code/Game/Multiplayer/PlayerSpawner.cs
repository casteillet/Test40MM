using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    public Transform frontSpawn;
    public Transform backSpawn;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObj = client.PlayerObject;
            var player = playerObj.GetComponent<Player>();

            var spawn = player.spawn.Value == SpawnPosition.Front
                ? frontSpawn
                : backSpawn;

            playerObj.transform.position = spawn.position;
        }
    }
}