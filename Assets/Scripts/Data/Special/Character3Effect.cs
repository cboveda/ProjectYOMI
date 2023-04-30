using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Character3Effect : CharacterBaseEffect
{
    private readonly float[] _outgoingDamageModifiers = { 0.8f, 1.0f, 1.2f, 1.5f};
    [SerializeField] int _modifierIndex;
    private bool _specialUsed;

    private void Awake()
    {
        _modifierIndex = 0;
        _specialUsed = false;
    }

    public override void DoSpecial(GameData context, ulong clientId)
    {
        // @TODO: needs work. This implementation will mess up turn history I think
        bool isPlayer1 = _playerCharacter.PlayerNumber == 1;
        var lastRound = context.RoundDataList.Last<RoundData>();
        var lastMove = isPlayer1 ? lastRound.MoveIdPlayer1 : lastRound.MoveIdPlayer2;
        _playerCharacter.PlayerData.Action = lastMove;
        _specialUsed = true;
    }

    public override float GetIncomingDamageModifier(GameData context, ulong clientId)
    {
        return 1.0f;
    }

    public override float GetOutgoingDamageModifier(GameData context, ulong clientId)
    {
        if (_specialUsed)
        {
            _specialUsed = false;
            return 2.0f;
        }
        
        var lastRound = context.RoundDataList.Last<RoundData>();
        bool isPlayer1 = _playerCharacter.PlayerNumber == 1;
        var currentMove = _playerCharacter.PlayerData.Action;
        if ((isPlayer1 ? lastRound.MoveIdPlayer1 : lastRound.MoveIdPlayer2) == currentMove)
        {
            ResetModifier();
        }
        else
        {
            IncrementModifier();
        }
        return _outgoingDamageModifiers[_modifierIndex];
    }

    public override float GetSpecialMeterGainModifier(GameData context, ulong clientId)
    {
        return 1.0f;
    }

    public override float GetSpecialMeterGivenModifier(GameData context, ulong clientId)
    {
        return 1.0f;
    }

    public void IncrementModifier()
    {
        _modifierIndex++;
        _modifierIndex = Mathf.Clamp(_modifierIndex, 0, _outgoingDamageModifiers.Length - 1);
    }

    public void ResetModifier()
    {
        _modifierIndex = 0;
    }
}
