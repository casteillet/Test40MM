using Unity.Netcode;
using UnityEngine;

public struct SpectatorPointerState : INetworkSerializable
{
    public Vector3 screenPosition;
    public bool isClicking;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref screenPosition);
        serializer.SerializeValue(ref isClicking);
    }
}