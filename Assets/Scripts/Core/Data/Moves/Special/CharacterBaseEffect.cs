using UnityEngine;

public abstract class CharacterBaseEffect : MonoBehaviour
{
    protected PlayerCharacter _playerCharacter;
    protected PlayerDataCollection _players;
    protected CombatEvaluator _combatEvaluator;
    protected Database _database;
    protected ITurnHistory _turnHistory;

    // TODO: Figure out Zenject factories
    public void Contstruct(PlayerCharacter playerCharacter, ITurnHistory turnHistory, PlayerDataCollection players, Database database, CombatEvaluator combatEvaluator)
    {
        _playerCharacter = playerCharacter;
        _turnHistory = turnHistory;
        _players = players;
        _database = database;
        _combatEvaluator = combatEvaluator;
    }

    public abstract void DoSpecial();
    public abstract float GetIncomingDamageModifier();
    public abstract float GetOutgoingDamageModifier();
    public abstract float GetSpecialMeterGainModifier();
    public abstract float GetSpecialMeterGivenModifier();


}
