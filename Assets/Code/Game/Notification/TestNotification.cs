using BennyKok.RuntimeDebug.Attributes;
using BennyKok.RuntimeDebug.Components;

public class TestNotification : RuntimeDebugBehaviour
{
    [DebugAction]
    public void ServerToAllClients()
    {
        var notification = new NotificationData
        {
            title = "Title test",
            message = "Message test",
            type = NotificationType.Success
        };

        NotificationManager.Instance.SendToAllClients(notification);
    }
}
