public class Character2Effect : CharacterBaseEffect
{
    public override void DoSpecial(bool didWinTurn)
    {
        if (!didWinTurn)
        {
            return;
        }
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

    public class ApplyLockout : CombatCommandBase
    {
        private readonly Move.Type _targetType;

        public ApplyLockout(ulong clientId, int round, Move.Type targetType) : base(clientId, round)
        {
            _targetType = targetType;
        }

        public override void Execute(CombatEvaluator context)
        {
            base.Execute(context);

            var targetPlayerCharacter = context.Players.GetByClientId(TargetClientId);
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

        public override void Execute(CombatEvaluator context)
        {
            base.Execute(context);

            var opponentPlayerCharacter = context.Players.GetByClientId(TargetClientId);
            if (_targetType != Move.Type.Special)
            {
                opponentPlayerCharacter.UsableMoveSet.EnableMoveByType(_targetType);
            }
        }
    }
}
