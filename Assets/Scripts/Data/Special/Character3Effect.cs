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
        // @TODO: needs work. This implementation will mess up turn history I think. Need a way to modify how a win is evaluated. For now, just does the double damage part.
        //bool isPlayer1 = _playerCharacter.PlayerNumber == 1;
        //var lastRound = context.TurnDataList[^1];
        //var lastMove = isPlayer1 ? lastRound.PlayerData1.Action : lastRound.PlayerData2.Action;
        //_playerCharacter.Action = lastMove;
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
        
        var lastRound = context.TurnDataList[^1];
        bool isPlayer1 = _playerCharacter.PlayerNumber == 1;
        var currentMove = _playerCharacter.PlayerData.Action;
        if ((isPlayer1 ? lastRound.PlayerData1.Action : lastRound.PlayerData2.Action) == currentMove)
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
