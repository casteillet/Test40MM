using System;
using Unity.Netcode;

public class NotificationManager : NetworkSingleton<NotificationManager>
{
    public static event Action<NotificationData> OnNotificationReceived;
    
    public void SendToAllClients(NotificationData notification)
    {
        if (!IsServer) return;

        ReceiveNotificationClientRpc(notification);
    }

    public void SendToClient(ulong clientId, NotificationData notification)
    {
        if (!IsServer) return;

        var rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { clientId }
            }
        };

        ReceiveNotificationClientRpc(notification, rpcParams);
    }
    
    [ClientRpc]
    private void ReceiveNotificationClientRpc(NotificationData notification, ClientRpcParams _ = default)
    {
        OnNotificationReceived?.Invoke(notification);
    }
}