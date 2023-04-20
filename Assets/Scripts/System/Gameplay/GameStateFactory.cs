using UnityEngine;

public class GameStateFactory
{
    protected GameStateMachine _context;

    public GameStateFactory(GameStateMachine currentContext)
    {
        _context = currentContext;
    }

    public GameBaseState NotReady()
    {
        return new GameNotReadyState(_context, this);
    }

    public GameBaseState Start()
    {
        return new GameStartState(_context, this);
    }

    public GameBaseState RoundActive()
    {
        return new GameRoundActiveState(_context, this);
    }

    public GameBaseState RoundResolve()
    {
        return new GameRoundResolveState(_context, this);
    }

    public GameBaseState End()
    {
        return new GameEndState(_context, this);
    }
}
