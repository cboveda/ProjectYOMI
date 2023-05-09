using Moq;
using NUnit.Framework;

public class GameStateFactoryTest
{
    GameStateFactory _gameStateFactory;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Mock<IGameStateMachine> gameStateMachineMock = new();
        _gameStateFactory = new(gameStateMachineMock.Object);
    }

    [Test]
    public void StartReturnsStartState()
    {
        Assert.That(_gameStateFactory.Start(), Is.TypeOf<GameStartState>());
    }

    [Test]
    public void TurnActiveReturnsTurnActiveState()
    {
        Assert.That(_gameStateFactory.TurnActive(), Is.TypeOf<GameTurnActiveState>());
    }

    [Test]
    public void TurnResolveReturnsTurnResolveState()
    {
        Assert.That(_gameStateFactory.TurnResolve(), Is.TypeOf<GameTurnResolveState>());
    }

    [Test]
    public void EndReturnsEndState()
    {
        Assert.That(_gameStateFactory.End(), Is.TypeOf<GameEndState>());
    }
}
