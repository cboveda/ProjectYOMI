using Unity.Netcode;
using UnityEngine;

public class PlayerData : ScriptableObject, INetworkSerializable
{
    private float _health;
    private float _specialMeter;
    private int _action;
    private int _comboCount;

    public float Health { get => _health; set => _health = value; }
    public float SpecialMeter { get => _specialMeter; set => _specialMeter = value; }
    public int Action { get => _action; set => _action = value; }
    public int ComboCount { get => _comboCount; set => _comboCount = value; }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _health);
        serializer.SerializeValue(ref _specialMeter);
        serializer.SerializeValue(ref _action);
        serializer.SerializeValue(ref _comboCount);
    }
}
