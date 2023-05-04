public class Character2Effect : CharacterBaseEffect
{
    public override void DoSpecial(GameData context, ulong clientId)
    {
        var opponentPlayerCharacter = context.GetPlayerCharacterByOpponentClientId(clientId);
        var opponentMoveId = opponentPlayerCharacter.PlayerData.Action;
        var opponentMove = _database.MoveDB.GetMoveById(opponentMoveId);
        var type = opponentMove ? opponentMove.MoveType : CharacterMove.Type.LightAttack; //What to do if the player didn't select? Picks LightAttack

        context.CombatCommands.Add(new ApplyLockout(clientId, context.RoundNumber, type));
        context.CombatCommands.Add(new ApplyLockout(clientId, context.RoundNumber + 1, type));
        context.CombatCommands.Add(new UndoLockout(clientId, context.RoundNumber + 2, type));
    }

    public override float GetIncomingDamageModifier(GameData context, ulong clientId)
    {
        return 1.0f;
    }

    public override float GetOutgoingDamageModifier(GameData context, ulong clientId)
    {
        return 1.0f;
    }

    public override float GetSpecialMeterGainModifier(GameData context, ulong clientId)
    {
        return 1.0f;
    }

    public override float GetSpecialMeterGivenModifier(GameData context, ulong clientId)
    {
        return 0.80f;
    }

    public class ApplyLockout : CombatCommandBase
    {
        private readonly CharacterMove.Type _targetType;

        public ApplyLockout(ulong clientId, int round, CharacterMove.Type targetType) : base(clientId, round)
        {
            _targetType = targetType;
        }

        public override void Execute(GameData context)
        {
            base.Execute(context);

            var opponentPlayerCharacter = context.GetPlayerCharacterByOpponentClientId(ClientId);
            opponentPlayerCharacter.UsableMoveSet.DisableMoveByType(_targetType);
        }
    }

    public class UndoLockout : CombatCommandBase
    {
        private readonly CharacterMove.Type _targetType;

        public UndoLockout(ulong clientId, int round, CharacterMove.Type targetType) : base(clientId, round)
        {
            _targetType = targetType;
        }

        public override void Execute(GameData context)
        {
            base.Execute(context);

            var opponentPlayerCharacter = context.GetPlayerCharacterByOpponentClientId(ClientId);
            if (_targetType != CharacterMove.Type.Special)
            {
                opponentPlayerCharacter.UsableMoveSet.EnableMoveByType(_targetType);
            }
        }
    }
}
