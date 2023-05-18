using UnityEngine;

public abstract class CharacterBaseEffect : MonoBehaviour
{
    protected CombatEvaluator _combatEvaluator;
    protected IDatabase _database;
    protected IPlayerDataCollection _players;
    protected ITurnHistory _turnHistory;
    protected PlayerCharacter _playerCharacter;

    public void Contstruct(PlayerCharacter playerCharacter, ITurnHistory turnHistory, IPlayerDataCollection players, IDatabase database, CombatEvaluator combatEvaluator)
    {
        _playerCharacter = playerCharacter;
        _turnHistory = turnHistory;
        _players = players;
        _database = database;
        _combatEvaluator = combatEvaluator;
    }

    public abstract void DoSpecial(bool didWinTurn);
}
