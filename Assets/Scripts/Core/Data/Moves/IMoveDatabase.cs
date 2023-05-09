public interface IMoveDatabase
{
    Move[] GetAllMoves();
    Move GetMoveById(int id);
    bool IsValidMoveId(int id);
}