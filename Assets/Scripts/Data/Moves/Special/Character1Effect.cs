using UnityEngine;

public class Character1Effect : CharacterBaseEffect
{
    [SerializeField] private float _healAmountOnSpecial = 15;

    public override void DoSpecial(GameData context, ulong clientId)
    {
        _playerCharacter.Health += _healAmountOnSpecial;
    }

    public override float GetIncomingDamageModifier(GameData context, ulong clientId)
    {
        return 1.0f;
    }

    public override float GetOutgoingDamageModifier(GameData context, ulong clientId)
    {
        var myMoveId = _playerCharacter.Action;
        var myMove = _database.MoveDB.GetMoveById(myMoveId);
        if (myMove != null && myMove.MoveType == CharacterMove.Type.Grab)
        {
            return 1.5f;
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
