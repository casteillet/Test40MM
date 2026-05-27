using Unity.Netcode;
using UnityEngine;

public struct SpectatorPointerState : INetworkSerializable
{
    public Vector2 ScreenPosition;
    public bool IsClicking;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ScreenPosition);
        serializer.SerializeValue(ref IsClicking);
    }
}