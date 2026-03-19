using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerLobbyState : INetworkSerializable, IEquatable<PlayerLobbyState>
{
    public FixedString64Bytes PlayerId;
    public ulong ClientId;
    public SpawnPosition Spawn;

    public bool Equals(PlayerLobbyState other)
    {
        return PlayerId.Equals(other.PlayerId);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref Spawn);
    }
}