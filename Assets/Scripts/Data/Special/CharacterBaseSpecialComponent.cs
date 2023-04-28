using UnityEngine;

public abstract class CharacterBaseSpecialComponent : MonoBehaviour
{
    public abstract void DoSpecial(GameData context, ulong clientId);
}
