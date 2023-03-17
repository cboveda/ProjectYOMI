using System;
using Unity.Netcode;

public struct CharacterSelectState : INetworkSerializable, IEquatable<CharacterSelectState>
{
    public ulong ClientId;
    public int CharacterId;

    public CharacterSelectState(ulong clientId, int characterId = -1)
    {
        this.ClientId = clientId;
        this.CharacterId = characterId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref CharacterId);
    }
    public bool Equals(CharacterSelectState other)
    {
        return ClientId == other.ClientId && 
            CharacterId == other.CharacterId;   
    }
}
