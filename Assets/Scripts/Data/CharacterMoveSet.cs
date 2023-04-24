using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterMoveSet", menuName = "Characters/Character Move Set")]
public class CharacterMoveSet : ScriptableObject
{
    [SerializeField] private CharacterMove _lightAttack;
    [SerializeField] private CharacterMove _heavyAttack;
    [SerializeField] private CharacterMove _parry;
    [SerializeField] private CharacterMove _grab;
    [SerializeField] private CharacterMove _special;

    public CharacterMove LightAttack => _lightAttack;
    public CharacterMove HeavyAttack => _heavyAttack;
    public CharacterMove Parry => _parry;
    public CharacterMove Grab => _grab;
    public CharacterMove Special => _special;

}
