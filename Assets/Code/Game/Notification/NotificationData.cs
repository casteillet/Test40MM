using Unity.Collections;
using Unity.Netcode;

public struct NotificationData : INetworkSerializable
{
    public FixedString128Bytes title;
    public FixedString512Bytes message;
    public NotificationType type;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref title);
        serializer.SerializeValue(ref message);
        serializer.SerializeValue(ref type);
    }
}