using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    private void OnEnable()
    {
        NotificationManager.OnNotificationReceived += ShowNotification;
    }

    private void OnDisable()
    {
        NotificationManager.OnNotificationReceived -= ShowNotification;
    }

    private void ShowNotification(NotificationData notification)
    {
        Debug.Log($"[{notification.type}] {notification.title}: {notification.message}");
    }
}