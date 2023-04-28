using UnityEngine;

public abstract class CharacterBaseEffect : MonoBehaviour
{
    public abstract void DoSpecial(GameData context, ulong clientId);

    public abstract float GetIncomingDamageModifier(GameData context, ulong clientId);
    public abstract float GetOutgoingDamageModifier(GameData context, ulong clientId);
    public abstract float GetSpecialMeterGainModifier(GameData context, ulong clientId);
    public abstract float GetSpecialMeterGivenModifier(GameData context, ulong clientId);

}
