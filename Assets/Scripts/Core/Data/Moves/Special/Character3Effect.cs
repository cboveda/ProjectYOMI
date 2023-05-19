using UnityEngine;

public class Character3Effect : CharacterBaseEffect
{
    private int _turnSpecialWasLastTriggeredOn = -10;

    public int TurnSpecialWasLastTriggeredOn { get => _turnSpecialWasLastTriggeredOn; set => _turnSpecialWasLastTriggeredOn = value; }

    public override void DoSpecial(bool didWinTurn)
    {
        var currentTurnNumber = _combatEvaluator.TurnNumber;
        if (SpecialCommandsAreAlreadyInEffect(currentTurnNumber))
        {
            return;
        }
        _turnSpecialWasLastTriggeredOn = currentTurnNumber;

        var myId = _playerCharacter.ClientId;
        _combatEvaluator.AddCombatCommand(new FreeSpecialUse(myId, currentTurnNumber, true));
        _combatEvaluator.AddCombatCommand(new FreeSpecialUse(myId, currentTurnNumber + 1, false));
        _combatEvaluator.AddCombatCommand(new ClearFreeSpecialUse(myId, currentTurnNumber + 2));
    }

    private bool SpecialCommandsAreAlreadyInEffect(int currentTurnNumber)
    {
        return currentTurnNumber - _turnSpecialWasLastTriggeredOn < 2;
    }

    public class FreeSpecialUse : CombatCommandBase
    {
        private readonly bool _isFirstInstance;

        public FreeSpecialUse(ulong targetClientId, int round, bool isFirstInstance = true) : base(targetClientId, round)
        {
            _isFirstInstance = isFirstInstance;
        }

        public override void Execute(CombatCommandExecutor context)
        {
            base.Execute(context);
            
            var targetPlayerCharacter = context.Players.GetByClientId(TargetClientId);
            var currentMoveType = context.Database.Moves.GetMoveById(targetPlayerCharacter.Action).MoveType;
            if (_isFirstInstance || currentMoveType != Move.Type.Special) 
            {
                targetPlayerCharacter.SpecialMeter = 100;
            }
        }
    }

    public class ClearFreeSpecialUse : CombatCommandBase
    {
        public ClearFreeSpecialUse(ulong targetClientId, int round) : base(targetClientId, round) { }

        public override void Execute(CombatCommandExecutor context)
        {
            var targetPlayerCharacter = context.Players.GetByClientId(TargetClientId);
            if (targetPlayerCharacter.SpecialMeter >= 100)
            {
                targetPlayerCharacter.SpecialMeter = 0;
            }
        }
    }
}
