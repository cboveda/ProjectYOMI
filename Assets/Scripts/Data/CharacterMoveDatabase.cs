using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Move Database", menuName = "Characters/Move Database")]
public class CharacterMoveDatabase : ScriptableObject
{
    [SerializeField] private CharacterMove[] moves = new CharacterMove[0];

    public CharacterMove[] GetAllMoves() => moves;
    public CharacterMove GetMoveById(int id)
    {

        foreach (var move in moves)
        {
            if (move.Id == id) return move;
        }
        return null;
    }

    public bool IsValidMoveId(int id)
    {
        return moves.Any(x => x.Id == id);
    }
}
