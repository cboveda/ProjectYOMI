using UnityEngine;
using Zenject;

public abstract class CharacterBaseEffect : MonoBehaviour
{
    protected PlayerCharacter _playerCharacter;
    protected Database _database;

    [Inject]
    public void Contstruct(Database database)
    {
        _database = database;
    }

    private void Awake()
    {
        _playerCharacter = GetComponent<PlayerCharacter>();
    }

    public abstract void DoSpecial(GameData context, ulong clientId);

    public abstract float GetIncomingDamageModifier(GameData context, ulong clientId);
    public abstract float GetOutgoingDamageModifier(GameData context, ulong clientId);
    public abstract float GetSpecialMeterGainModifier(GameData context, ulong clientId);
    public abstract float GetSpecialMeterGivenModifier(GameData context, ulong clientId);

}
