using UnityEngine;

public abstract class CharacterBaseEffect : MonoBehaviour, ICharacterBaseEffect
{
    protected CombatCommandExecutor _combatEvaluator;
    protected IDatabase _database;
    protected IPlayerDataCollection _players;
    protected ITurnHistory _turnHistory;
    protected IPlayerCharacter _playerCharacter;

    public void Construct(
        IPlayerCharacter playerCharacter,
        ITurnHistory turnHistory,
        IPlayerDataCollection players,
        IDatabase database,
        CombatCommandExecutor combatEvaluator)
    {
        _playerCharacter = playerCharacter;
        _turnHistory = turnHistory;
        _players = players;
        _database = database;
        _combatEvaluator = combatEvaluator;
    }

    public abstract void DoSpecial(bool didWinTurn);
}
