using UnityEngine;

public class LocalNotificationManager : Singleton<LocalNotificationManager>
{
    [SerializeField] private ToastNotificationUI toastNotificationUI;

    public void Show(LocalNotification notification) => toastNotificationUI.Show(notification);
}