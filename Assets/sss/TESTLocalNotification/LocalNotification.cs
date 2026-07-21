public struct LocalNotification
{
    public readonly string Title;
    public readonly string Message;
    public readonly NotificationType Type;
    public readonly float Duration;

    public LocalNotification(string title, string message, NotificationType type, float duration = 1)
    {
        Title = title;
        Message = message;
        Type = type;
        Duration = duration;
    }
}
