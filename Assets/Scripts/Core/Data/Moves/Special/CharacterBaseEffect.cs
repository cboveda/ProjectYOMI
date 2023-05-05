using UnityEngine;
using Zenject;

public abstract class CharacterBaseEffect : MonoBehaviour
{
    protected PlayerDataCollection _players;
    protected PlayerCharacter _playerCharacter;
    protected CombatEvaluator _combatEvaluator;
    protected Database _database;
    protected GameData _gameData;

    [Inject]
    public void Contstruct(PlayerDataCollection players, Database database, GameData gameData, CombatEvaluator combatEvaluator)
    {
        _players = players;
        _database = database;
        _gameData = gameData;
        _combatEvaluator = combatEvaluator;
    }

    private void Awake()
    {
        _playerCharacter = GetComponent<PlayerCharacter>();
    }

    public abstract void DoSpecial();

    public abstract float GetIncomingDamageModifier();
    public abstract float GetOutgoingDamageModifier();
    public abstract float GetSpecialMeterGainModifier();
    public abstract float GetSpecialMeterGivenModifier();

}
