using System;
using Unity.Netcode;

public class UsableMoveSet : NetworkBehaviour
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
        foreach (CharacterMove.Type type in Enum.GetValues(typeof(CharacterMove.Type)))
        {
            byte isUsable = (byte)(moveSet.GetMoveByType(type).UsableByDefault ? 1 : 0);
            byte typeAsByte = (byte)type;
            _moves.Value |= (byte)(typeAsByte * isUsable);
        }
    }

    public void DisableMoveByType(CharacterMove.Type type)
    {
        byte mask = (byte) (Byte.MaxValue - (byte)type);
        _moves.Value &= mask;
    }

    public void EnableMoveByType(CharacterMove.Type type)
    {
        byte mask = (byte) type;
        _moves.Value |= mask;
    }

    public bool CheckEnabledByType(CharacterMove.Type type)
    {
        byte mask = (byte) type;
        return (_moves.Value & mask) == mask;
    }
}
