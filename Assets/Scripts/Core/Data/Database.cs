using UnityEngine;

public class Database : MonoBehaviour, IDatabase
{
    [SerializeField] private CharacterDatabase _characterDatabase;
    [SerializeField] private MoveDatabase _moveDatabase;
    [SerializeField] private MoveInteractions _moveInteractions;

    public ICharacterDatabase Characters { get => _characterDatabase; }
    public IMoveDatabase Moves { get => _moveDatabase; }
    public IMoveInteractions MoveInteractions { get => _moveInteractions; }

}
