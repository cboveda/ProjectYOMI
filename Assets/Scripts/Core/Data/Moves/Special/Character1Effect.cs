using UnityEngine;

public class Character1Effect : CharacterBaseEffect
{
    [SerializeField] private float _healAmountOnSpecial = 15;

    public override void DoSpecial()
    {
        _playerCharacter.Health += _healAmountOnSpecial;
    }

    public override float GetIncomingDamageModifier()
    {
        return 1.0f;
    }

    public override float GetOutgoingDamageModifier()
    {
        var myMoveId = _playerCharacter.Action;
        Debug.Log($"Is database null? {_database == null}");
        Debug.Log($"Is database.Moves null? {_database.Moves}");

        var myMove = _database.Moves.GetMoveById(myMoveId);
        if (myMove != null && myMove.MoveType == Move.Type.Grab)
        {
            return 1.5f;
        }
        return 1.0f;
    }

    public override float GetSpecialMeterGainModifier()
    {
        return 1.0f;
    }

    public override float GetSpecialMeterGivenModifier()
    {
        return 1.0f;
    }
}
