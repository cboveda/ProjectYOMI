using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct TurnResult : INetworkSerializable, IEquatable<TurnResult>
{
    public int TurnNumber;
    public PlayerData PlayerData1;
    public PlayerData PlayerData2;
    public float DamageToPlayer1;
    public float DamageToPlayer2;
    public Move.Type Player1NextComboMove;
    public Move.Type Player2NextComboMove;
    public FixedString32Bytes Summary;

    public TurnResult (
        int turnNumber,
        PlayerData playerData1,
        PlayerData playerData2,
        float damageToPlayer1,
        float damageToPlayer2,
        FixedString32Bytes summary,
        Move.Type player1NextComboMove = Move.Type.None,
        Move.Type player2NextComboMove = Move.Type.None)
    {
        TurnNumber = turnNumber;
        PlayerData1 = playerData1;
        PlayerData2 = playerData2;
        DamageToPlayer1 = damageToPlayer1;
        DamageToPlayer2 = damageToPlayer2;
        Summary = summary;
        Player1NextComboMove = player1NextComboMove;
        Player2NextComboMove = player2NextComboMove;

    }

    public bool Equals(TurnResult other)
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
        serializer.SerializeValue(ref Player1NextComboMove);
        serializer.SerializeValue(ref Player2NextComboMove);
    }

    public override string ToString()
    {
        return $"[Turn {TurnNumber}]\nP1: {PlayerData1}\nP2: {PlayerData2}\nP1 Combo: {Player1NextComboMove}, P2 Combo: {Player2NextComboMove}";
    }
}
