using System;
using Unity.Netcode;

public class UsableMoveSet : NetworkBehaviour, IUsableMoveSet
{
    private NetworkVariable<byte> _moves = new();
    public NetworkVariable<byte> Moves { get => _moves; }

    public override void OnNetworkSpawn()
    {
        var moveSet = GetComponent<PlayerCharacter>().Character.CharacterMoveSet;
        InitializeMoveSet(moveSet);
    }

    public void InitializeMoveSet(CharacterMoveSet moveSet)
    {
        foreach (Move.Type type in Enum.GetValues(typeof(Move.Type)))
        {
            var move = moveSet.GetMoveByType(type);
            if (move == null)
            {
                continue;
            }
            byte isUsable = (byte)(moveSet.GetMoveByType(type).UsableByDefault ? 1 : 0);
            byte typeAsByte = (byte)type;
            _moves.Value |= (byte)(typeAsByte * isUsable);
        }
    }

    public void DisableMoveByType(Move.Type type)
    {
        byte mask = (byte)(Byte.MaxValue - (byte)type);
        _moves.Value &= mask;
    }

    public void EnableMoveByType(Move.Type type)
    {
        byte mask = (byte)type;
        _moves.Value |= mask;
    }

    public bool CheckEnabledByType(Move.Type type)
    {
        byte mask = (byte)type;
        return (_moves.Value & mask) == mask;
    }
}
