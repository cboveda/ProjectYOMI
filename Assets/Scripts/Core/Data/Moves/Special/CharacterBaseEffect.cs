using UnityEngine;

public abstract class CharacterBaseEffect : MonoBehaviour
{
    protected PlayerCharacter _playerCharacter;
    protected PlayerDataCollection _players;
    protected CombatEvaluator _combatEvaluator;
    protected Database _database;
    protected GameData _gameData;

    // TODO: Figure out Zenject factories
    public void Contstruct(PlayerCharacter playerCharacter, GameData gameData, PlayerDataCollection players, Database database, CombatEvaluator combatEvaluator)
    {
        _playerCharacter = playerCharacter;
        _gameData = gameData;
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
