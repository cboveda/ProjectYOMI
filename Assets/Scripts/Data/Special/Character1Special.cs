using UnityEngine;

public class Character1Special : CharacterBaseSpecialComponent
{
    public override void DoSpecial(GameData context, ulong clientId)
    {
        Debug.Log("Did special stuff");
    }
}
