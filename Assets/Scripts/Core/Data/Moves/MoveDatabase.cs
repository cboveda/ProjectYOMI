using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Move Database", menuName = "Moves/Move Database")]
public class MoveDatabase : ScriptableObject, IMoveDatabase
{
    [SerializeField] private Move[] _moves = new Move[0];

    public Move[] GetAllMoves() => _moves;
    public Move GetMoveById(int id)
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
