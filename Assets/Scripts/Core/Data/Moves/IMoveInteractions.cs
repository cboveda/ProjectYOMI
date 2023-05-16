using System.Collections.Generic;

public interface IMoveInteractions
{
    List<Move.Type> DefeatedByType(Move.Type type);
    List<Move.Type> DefeatsType(Move.Type type);
}