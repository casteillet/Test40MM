using Unity.Netcode;
using UnityEngine;

public struct SpectatorPointerState : INetworkSerializable
{
    public Vector3 viewportPosition;
    public PointerEventType eventType;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref viewportPosition);
        serializer.SerializeValue(ref eventType);
    }
}