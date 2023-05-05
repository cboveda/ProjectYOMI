using UnityEngine;

public class Database : MonoBehaviour
{
    [SerializeField] private CharacterDatabase _characterDatabase;
    [SerializeField] private MoveDatabase _moveDatabase;

    public CharacterDatabase Characters { get => _characterDatabase; }
    public MoveDatabase Moves { get => _moveDatabase; }
}
