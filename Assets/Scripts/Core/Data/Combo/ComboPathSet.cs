using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterComboPathSet", menuName = "Characters/Combo Path Set")]
public class ComboPathSet : SerializableDictionary<Move.Type, ComboPath>
{
}
