using UnityEngine;
using System;

public class UsableMoveSet : ScriptableObject
{
    private byte _moves;
    public byte Moves { get => _moves; }

    public void Initialize(CharacterMoveSet moveSet)
    {
        foreach (CharacterMove.Type type in Enum.GetValues(typeof(CharacterMove.Type)))
        {
            byte isUsable = (byte) (moveSet.GetMoveByType(type).UsableByDefault ? 1 : 0);
            byte typeAsByte = (byte) type;
            _moves |= (byte) (typeAsByte * isUsable);
        }
    }

    public void DisableMoveByType(CharacterMove.Type type)
    {
        byte mask = (byte) (Byte.MaxValue - (byte)type);
        _moves &= mask;
    }

    public void EnableMoveByType(CharacterMove.Type type)
    {
        byte mask = (byte) type;
        _moves |= mask;
    }

    public bool CheckEnabledByType(CharacterMove.Type type)
    {
        byte mask = (byte) type;
        return (_moves & mask) == mask;
    }
}