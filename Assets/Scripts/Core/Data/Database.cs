using UnityEngine;

public class Database : MonoBehaviour, IDatabase
{
    [SerializeField] private CharacterDatabase _characterDatabase;
    [SerializeField] private MoveDatabase _moveDatabase;

    public ICharacterDatabase Characters { get => _characterDatabase; }
    public IMoveDatabase Moves { get => _moveDatabase; }
}
