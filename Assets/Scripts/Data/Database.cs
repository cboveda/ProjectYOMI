using UnityEngine;

public class Database : MonoBehaviour
{
    [SerializeField] private CharacterDatabase _characterDatabase;
    [SerializeField] private CharacterMoveDatabase _characterMoveDatabase;

    public CharacterDatabase CharacterDB { get => _characterDatabase; }
    public CharacterMoveDatabase MoveDB { get => _characterMoveDatabase; }
}
