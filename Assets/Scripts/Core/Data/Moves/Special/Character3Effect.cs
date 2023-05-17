public class Character3Effect : CharacterBaseEffect
{
    private void Awake()
    {
    }

    public override void DoSpecial(bool didWinTurn)
    {
        var myId = _playerCharacter.ClientId;
        var currentTurnNumber = _combatEvaluator.TurnNumber;
        _combatEvaluator.AddCombatCommand(new FreeSpecialUse(myId, currentTurnNumber, true));
        _combatEvaluator.AddCombatCommand(new FreeSpecialUse(myId, currentTurnNumber + 1, false));
    }

    public class FreeSpecialUse : CombatCommandBase
    {
        private readonly bool _isFirstInstance;

        public FreeSpecialUse(ulong targetClientId, int round, bool isFirstInstance = true) : base(targetClientId, round)
        {
            _isFirstInstance = isFirstInstance;
        }

        public override void Execute()
        {
            base.Execute();
            
            var targetPlayerCharacter = _players.GetByClientId(TargetClientId);

            if (_isFirstInstance || targetPlayerCharacter.Action != (int) Move.Type.Special)
            {
                targetPlayerCharacter.SpecialMeter = 100;
            }
        }
    }
}
