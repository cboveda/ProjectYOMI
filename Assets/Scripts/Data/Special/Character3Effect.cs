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
        bool isPlayer1 = clientId == context.ClientIdPlayer1.Value;
        var lastRound = context.RoundDataList.Last<RoundData>();
        var lastMove = isPlayer1 ? lastRound.MoveIdPlayer1 : lastRound.MoveIdPlayer2;
        if (isPlayer1)
        {
            context.ActionPlayer1.Value = lastMove;
        }
        else
        {
            context.ActionPlayer2.Value = lastMove;
    }
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
        
        var lastThreeRounds = context.RoundDataList.Reverse<RoundData>().Take(2);
        bool isPlayer1 = clientId == context.ClientIdPlayer1.Value;
        var currentMove = isPlayer1 ? context.ActionPlayer1.Value : context.ActionPlayer2.Value;
        if (lastThreeRounds.Any(round => (isPlayer1 ? round.MoveIdPlayer1 : round.MoveIdPlayer2) == currentMove))
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
