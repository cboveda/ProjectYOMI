using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct TurnData : INetworkSerializable, IEquatable<TurnData>
{
    public int TurnNumber;
    public PlayerData PlayerData1;
    public PlayerData PlayerData2;
    public float DamageToPlayer1;
    public float DamageToPlayer2;
    public FixedString32Bytes Summary;

    public TurnData (
        int turnNumber,
        PlayerData playerData1,
        PlayerData playerData2,
        float damageToPlayer1,
        float damageToPlayer2,
        FixedString32Bytes summary)
    {
        TurnNumber = turnNumber;
        PlayerData1 = playerData1;
        PlayerData2 = playerData2;
        DamageToPlayer1 = damageToPlayer1;
        DamageToPlayer2 = damageToPlayer2;
        Summary = summary;
    }

    public bool Equals(TurnData other)
    {
        return TurnNumber == other.TurnNumber;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref TurnNumber);
        serializer.SerializeValue(ref PlayerData1);
        serializer.SerializeValue(ref PlayerData2);
        serializer.SerializeValue(ref DamageToPlayer1);
        serializer.SerializeValue(ref DamageToPlayer2);
        serializer.SerializeValue(ref Summary);
    }

    public override string ToString()
    {
        return $"[Turn {TurnNumber}]\nP1: {PlayerData1}, DmgTaken: {DamageToPlayer1}\nP2: {PlayerData2}, DmgTaken: {DamageToPlayer2}";
    }
}
