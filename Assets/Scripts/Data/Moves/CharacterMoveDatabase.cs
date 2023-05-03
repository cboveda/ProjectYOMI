using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Move Database", menuName = "Characters/Move Database")]
public class CharacterMoveDatabase : ScriptableObject
{
    [SerializeField] private CharacterMove[] _moves = new CharacterMove[0];

    public CharacterMove[] GetAllMoves() => _moves;
    public CharacterMove GetMoveById(int id)
    {

        foreach (var move in _moves)
        {
            if (move.Id == id) return move;
        }
        return null;
    }

    public bool IsValidMoveId(int id)
    {
        return _moves.Any(x => x.Id == id);
    }
}
