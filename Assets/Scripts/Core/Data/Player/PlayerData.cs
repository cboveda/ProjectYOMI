using System;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
{
    public float Health;
    public float SpecialMeter;
    public int Action;
    public int ComboCount;
    public int Position;
    public bool ComboIsFresh;

    public PlayerData(float health, float specialMeter = 0, int action = -1, int comboCount = 0, int position = 0, bool comboIsFresh = true)
    {
        Health = health;
        SpecialMeter = specialMeter;
        Action = action;
        ComboCount = comboCount;
        Position = position;
        ComboIsFresh = comboIsFresh;
    }

    public bool Equals(PlayerData other)
    {
        return Health == other.Health && 
            SpecialMeter == other.SpecialMeter && 
            Action == other.Action && 
            ComboCount == other.ComboCount &&
            Position == other.Position &&
            ComboIsFresh == other.ComboIsFresh;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Health);
        serializer.SerializeValue(ref SpecialMeter);
        serializer.SerializeValue(ref Action);
        serializer.SerializeValue(ref ComboCount);
        serializer.SerializeValue(ref Position);
        serializer.SerializeValue(ref ComboIsFresh);
    }

    public override string ToString()
    {
        return $"{{Health: {Health}, Special: {SpecialMeter}, Action: {Action}, Combo: {ComboCount}, Postion: {Position}, Fresh: {ComboIsFresh}}}";
    }
}
