using Unity.Netcode;
using UnityEngine;

public class ClientPointerSender : NetworkBehaviour
{
    private void Update()
    {
        //if (!IsOwner || IsServer) return;
        if (!IsOwner) return;

        var state = new SpectatorPointerState
        {
            ScreenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y),
            IsClicking = Input.GetMouseButton(0)
        };

        SendPointerServerRpc(state);
        Debug.Log($"ClientPointerSender: {state}");
    }

    [ServerRpc]
    private void SendPointerServerRpc(SpectatorPointerState state)
    {
        SpectatorManager.Instance.UpdatePointer(OwnerClientId, state);
        Debug.Log($"SendPointerServerRpc: {state}");
    }
}