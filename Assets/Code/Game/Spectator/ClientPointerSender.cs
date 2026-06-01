using Unity.Netcode;
using UnityEngine;

public class ClientPointerSender : MonoBehaviour
{
    private void Update()
    {
        if (NetworkManager.Singleton.IsServer) return;

        var state = new SpectatorPointerState
        {
            screenPosition = Input.mousePosition,
            clicked = Input.GetMouseButtonDown(0)
        };

        SpectatorManager.Instance.UpdatePointerServerRpc(NetworkManager.Singleton.LocalClientId, state);
        
        // Debug.Log($"ClientPointerSender: {state.ScreenPosition}, {state.IsClicking}");
    }
}