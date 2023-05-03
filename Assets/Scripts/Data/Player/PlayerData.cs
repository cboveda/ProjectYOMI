using System;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
{
    public float Health;
    public float SpecialMeter;
    public int Action;
    public int ComboCount;

    public PlayerData(float health, float specialMeter = 0, int action = -1, int comboCount = 0)
    {
        Health = health;
        SpecialMeter = specialMeter;
        Action = action;
        ComboCount = comboCount;
    }

    public bool Equals(PlayerData other)
    {
        return Health == other.Health && 
            SpecialMeter == other.SpecialMeter && 
            Action == other.Action && 
            ComboCount == other.ComboCount;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Health);
        serializer.SerializeValue(ref SpecialMeter);
        serializer.SerializeValue(ref Action);
        serializer.SerializeValue(ref ComboCount);
    }

    public override string ToString()
    {
        return $"{{He: {Health}, Sp: {SpecialMeter}, Ac: {Action}, Cm: {ComboCount}}}";
    }
}
