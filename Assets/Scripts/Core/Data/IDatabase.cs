﻿public interface IDatabase
{
    ICharacterDatabase Characters { get; }
    IMoveDatabase Moves { get; }
    IMoveInteractions MoveInteractions { get; }
}