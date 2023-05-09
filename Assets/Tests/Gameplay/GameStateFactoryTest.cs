using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameStateFactoryTest
{
    GameStateFactory gameStateFactory;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Mock<IGameStateMachine> gameStateMachineMock = new();
        gameStateFactory = new(gameStateMachineMock.Object);
    }

    [Test]
    public void StartReturnsStartState()
    {
        Assert.AreSame(typeof(GameStartState), gameStateFactory.Start());
    }
}
