using UnityEngine;

public class Character2Special : CharacterBaseSpecialComponent
{
    public override void DoSpecial(GameData context, ulong clientId)
    {
        //Lock out the opponents move from last round and nullify all damage
        //Add a command to the queue in GameData to be executed next round to also lock out the same move
    }
}
