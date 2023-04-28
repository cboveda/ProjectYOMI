using System.Linq;
using UnityEngine;

public class Character1Effect : CharacterBaseEffect
{
    public override void DoSpecial(GameData context, ulong clientId)
    {
        bool isPlayer1 = clientId == context.ClientIdPlayer1.Value;
        if (isPlayer1)
        {
            context.HealthPlayer1.Value += 15;
        }
        else
        {
            context.HealthPlayer2.Value += 15;
        }
    }

    public override float GetIncomingDamageModifier(GameData context, ulong clientId)
    {
        return 1.0f;
    }

    public override float GetOutgoingDamageModifier(GameData context, ulong clientId)
    {
        bool isPlayer1 = clientId == context.ClientIdPlayer1.Value;
        var myMove = context.CharacterMoveDatabase.GetMoveById(isPlayer1 ? context.ActionPlayer1.Value : context.ActionPlayer2.Value);
        if (myMove != null)
        {
            if (myMove.MoveType == CharacterMove.Type.Grab)
            {
                return 1.5f;
            }
        }
        return 1.0f;
    }

    public override float GetSpecialMeterGainModifier(GameData context, ulong clientId)
    {
        return 1.0f;
    }

    public override float GetSpecialMeterGivenModifier(GameData context, ulong clientId)
    {
        return 1.0f;
    }
}
