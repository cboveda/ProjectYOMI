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

    public GameBaseState TurnActive()
    {
        return new GameTurnActiveState(_context, this);
    }

    public GameBaseState TurnResolve()
    {
        return new GameTurnResolveState(_context, this);
    }

    public GameBaseState End()
    {
        return new GameEndState(_context, this);
    }
}