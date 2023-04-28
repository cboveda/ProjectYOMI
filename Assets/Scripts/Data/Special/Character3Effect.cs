using System.Linq;
using UnityEngine;

public class Character3Effect : CharacterBaseEffect
{
    private readonly float[] _outgoingDamageModifiers = { 0.8f, 1.0f, 1.2f, 1.5f };
    private int _modifierIndex = 0;

    public override void DoSpecial(GameData context, ulong clientId)
    {
        
    }

    public override float GetIncomingDamageModifier(GameData context, ulong clientId)
    {
        return 1.0f;
    }

    public override float GetOutgoingDamageModifier(GameData context, ulong clientId)
    {
        var lastThreeRounds = context.RoundDataList.Reverse<RoundData>().Take(3);
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

    private void IncrementModifier()
    {
        _modifierIndex = Mathf.Clamp(_modifierIndex++, 0, _outgoingDamageModifiers.Length);
    }

    private void ResetModifier()
    {
        _modifierIndex = 0;
    }
}
