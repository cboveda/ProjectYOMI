using Zenject;

public class Character2Effect : CharacterBaseEffect
{
    public override void DoSpecial()
    {
        var myId = _playerCharacter.ClientId;
        var opponentPlayerCharacter = _players.GetByOpponentClientId(myId);
        var opponentClientId = opponentPlayerCharacter.ClientId;
        var opponentMoveId = opponentPlayerCharacter.PlayerData.Action;
        var opponentMove = _database.Moves.GetMoveById(opponentMoveId);
        var opponentMoveType = opponentMove ? opponentMove.MoveType : Move.Type.LightAttack; //What to do if the player didn't select? Picks LightAttack
        var currentTurnNumber = _combatEvaluator.TurnNumber;

        _combatEvaluator.AddCombatCommand(new ApplyLockout(opponentClientId, currentTurnNumber, opponentMoveType));
        _combatEvaluator.AddCombatCommand(new ApplyLockout(opponentClientId, currentTurnNumber + 1, opponentMoveType));
        _combatEvaluator.AddCombatCommand(new UndoLockout(opponentClientId, currentTurnNumber + 2, opponentMoveType));
    }

    public override float GetIncomingDamageModifier()
    {
        return 1.0f;
    }

    public override float GetOutgoingDamageModifier()
    {
        return 1.0f;
    }

    public override float GetSpecialMeterGainModifier()
    {
        return 1.0f;
    }

    public override float GetSpecialMeterGivenModifier()
    {
        return 0.80f;
    }

    public class ApplyLockout : CombatCommandBase
    {
        private readonly Move.Type _targetType;

        public ApplyLockout(ulong clientId, int round, Move.Type targetType) : base(clientId, round)
        {
            _targetType = targetType;
        }

        public override void Execute()
        {
            base.Execute();

            var targetPlayerCharacter = _players.GetByClientId(TargetClientId);
            targetPlayerCharacter.UsableMoveSet.DisableMoveByType(_targetType);
        }
    }

    public class UndoLockout : CombatCommandBase
    {
        private readonly Move.Type _targetType;

        public UndoLockout(ulong clientId, int round, Move.Type targetType) : base(clientId, round)
        {
            _targetType = targetType;
        }

        public override void Execute()
        {
            base.Execute();

            var opponentPlayerCharacter = _players.GetByClientId(TargetClientId);
            if (_targetType != Move.Type.Special)
            {
                opponentPlayerCharacter.UsableMoveSet.EnableMoveByType(_targetType);
            }
        }
    }
}
