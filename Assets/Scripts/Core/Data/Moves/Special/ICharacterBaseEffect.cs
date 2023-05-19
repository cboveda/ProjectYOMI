public interface ICharacterBaseEffect
{
    void Construct(IPlayerCharacter playerCharacter, ITurnHistory turnHistory, IPlayerDataCollection players, IDatabase database, CombatCommandExecutor combatEvaluator);
    void DoSpecial(bool didWinTurn);
}