using System.Collections.Generic;

public interface IMoveInteractions
{
    Dictionary<Move.Type, List<Move.Type>> DefeatedByType { get; }
    Dictionary<Move.Type, List<Move.Type>> DefeatsType { get; }
    Move.Type[] SpecialDefeatsTypes { get; set; }
}