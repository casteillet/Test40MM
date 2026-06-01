using Unity.Netcode;
using UnityEngine;

public class ClientPointerSender : NetworkBehaviour
{
    private bool isServerListening;
    
    private Vector2 lastViewportPosition = new(-1f, -1f);
    
    private void Update()
    {
        if (!IsOwner) return;
        
        if (!isServerListening) return;
        
        SendMove();
        
        if (Input.GetMouseButtonDown(0))
        {
            SendPointerEvent(PointerEventType.Down);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            SendPointerEvent(PointerEventType.Up);
        }
    }
    
    private void SendMove()
    {
        var viewportPosition = GetViewportPosition();

        if (viewportPosition == lastViewportPosition) return;

        lastViewportPosition = viewportPosition;
        
        SendPointerEvent(PointerEventType.Move, viewportPosition);
    }

    private void SendPointerEvent(PointerEventType eventType)
    {
        SendPointerEvent(eventType, GetViewportPosition());
    }
    
    private void SendPointerEvent(PointerEventType eventType, Vector2 viewportPosition)
    {
        SpectatorPointerState state = new()
        {
            viewportPosition = viewportPosition,
            eventType = eventType
        };

        SpectatorManager.Instance.UpdatePointerServerRpc(state);
    }

    private static Vector2 GetViewportPosition()
    {
        return new Vector2(
            Input.mousePosition.x / Screen.width,
            Input.mousePosition.y / Screen.height);
    }
    
    [ClientRpc]
    public void SetSpectatorListeningStateClientRpc(bool state)
    {
        isServerListening = state;
    }
}