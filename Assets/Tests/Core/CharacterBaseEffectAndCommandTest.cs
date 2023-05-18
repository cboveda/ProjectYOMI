using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine.TestTools;
using UnityEngine;

public class CharacterBaseEffectAndCommandTest
{
    GameObject _testObject;

    Character1Effect _effect1;
    Character2Effect _effect2;
    Character3Effect _effect3;

    Mock<IPlayerCharacter> _playerCharacter;
    Mock<ITurnHistory> _turnHistory;
    Mock<IPlayerDataCollection> _players;
    Mock<IDatabase> _database;
    Mock<CombatEvaluator> _combatEvaluator;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _testObject = new GameObject();

        _testObject.AddComponent<Character1Effect>();
        _testObject.AddComponent<Character2Effect>();
        _testObject.AddComponent<Character3Effect>();

        _effect1 = _testObject.GetComponent<Character1Effect>();
        _effect2 = _testObject.GetComponent<Character2Effect>();
        _effect3 = _testObject.GetComponent<Character3Effect>();

        _playerCharacter = new();
        _turnHistory = new();
        _players = new();
        _database = new();
        _combatEvaluator = new();

        _effect1.Contstruct(
            playerCharacter: _playerCharacter.Object,
            turnHistory: _turnHistory.Object,
            players: _players.Object,
            database: _database.Object,
            combatEvaluator: _combatEvaluator.Object);
        _effect2.Contstruct(
            playerCharacter: _playerCharacter.Object,
            turnHistory: _turnHistory.Object,
            players: _players.Object,
            database: _database.Object,
            combatEvaluator: _combatEvaluator.Object);
        _effect3.Contstruct(
            playerCharacter: _playerCharacter.Object,
            turnHistory: _turnHistory.Object,
            players: _players.Object,
            database: _database.Object,
            combatEvaluator: _combatEvaluator.Object);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Character1EffectIncreaseOpponentKnockbackOnWin(bool didWinTurn)
    {
        ulong myId = 0;
        _playerCharacter.SetupProperty(m => m.ClientId, myId);
        _playerCharacter.SetupProperty(m => m.Position, 0);
        var opponentMock = new Mock<IPlayerCharacter>();
        opponentMock.SetupProperty(m => m.Position, 0);
        _players.Setup(m => m.GetByOpponentClientId(myId)).Returns(opponentMock.Object);

        var expectedOpponentValue = (didWinTurn) ? -1 : 0;
        var expectedValue = (didWinTurn) ? 1 : 0;

        _effect1.DoSpecial(didWinTurn);
        Assert.AreEqual(expectedOpponentValue, opponentMock.Object.Position);
        Assert.AreEqual(expectedValue, _playerCharacter.Object.Position);
    }

    // Was it worth it?
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Character2EffectAddsLockoutCommandsOnWin(bool didWinTurn)
    {
        // Test specific setup and mocking
        ulong myId = 0;
        int calls = 0;
        var callArgs = new List<CombatCommandBase>();
        var opponentMock = new Mock<IPlayerCharacter>();
        var opponentUsableMoveMock = new Mock<IUsableMoveSet>();

        _playerCharacter.SetupProperty(m => m.ClientId, myId);
        opponentMock.SetupProperty(m => m.ClientId)
            .SetReturnsDefault(1);
        opponentMock.SetupProperty(m => m.PlayerData)
            .SetReturnsDefault(new PlayerData());
        opponentUsableMoveMock.Setup(m => m.DisableMoveByType(It.IsAny<Move.Type>()))
            .Callback(() => calls++)
            .Verifiable();
        opponentUsableMoveMock.Setup(m => m.EnableMoveByType(It.IsAny<Move.Type>()))
            .Callback(() => calls++)
            .Verifiable();
        opponentMock.Setup(m => m.UsableMoveSet)
            .Returns(opponentUsableMoveMock.Object);
        _combatEvaluator.Setup(m => m.Players)
            .Returns(_players.Object);
        _players.Setup(m => m.GetByOpponentClientId(myId))
            .Returns(opponentMock.Object);
        _players.Setup(m => m.GetByClientId(opponentMock.Object.ClientId))
            .Returns(opponentMock.Object);
        _database.Setup(m => m.Moves.GetMoveById(It.IsAny<int>()));
        _combatEvaluator.Setup(m => m.TurnNumber)
            .Returns(0);
        _combatEvaluator.Setup(m => m.AddCombatCommand(It.IsAny<CombatCommandBase>()))
            .Callback((CombatCommandBase c) => callArgs.Add(c));

        // Do the thing
        _effect2.DoSpecial(didWinTurn);

        // General assertions
        Assert.AreEqual(didWinTurn ? 3 : 0, callArgs.Count);

        if (!didWinTurn)
        {
            return;
        }

        // Specific assertions if didWinTurn
        Assert.AreEqual(0, callArgs[0].Round);
        callArgs[0].Execute(_combatEvaluator.Object);
        Assert.AreEqual(1, calls);
        Assert.IsInstanceOf<Character2Effect.ApplyLockout>(callArgs[0]);

        Assert.AreEqual(1, callArgs[1].Round);
        callArgs[1].Execute(_combatEvaluator.Object);
        Assert.AreEqual(2, calls);
        Assert.IsInstanceOf<Character2Effect.ApplyLockout>(callArgs[1]);

        Assert.AreEqual(2, callArgs[2].Round);
        callArgs[2].Execute(_combatEvaluator.Object);
        Assert.AreEqual(3, calls);
        Assert.IsInstanceOf<Character2Effect.UndoLockout>(callArgs[2]);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void Character3EffectModifiesSpecialMeterForTwoTurns(bool didWinTurn)
    {
        ulong myId = 0;
        int turn = 0;
        var callArgs = new List<CombatCommandBase>();

        _combatEvaluator.Setup(m => m.TurnNumber)
            .Returns(turn);
        _playerCharacter.SetupProperty(m => m.ClientId, myId);
        _playerCharacter.SetupProperty(m => m.SpecialMeter, 0);
        _combatEvaluator.Setup(m => m.AddCombatCommand(It.IsAny<CombatCommandBase>()))
            .Callback((CombatCommandBase c) => callArgs.Add(c));
        _combatEvaluator.Setup(m => m.Players)
            .Returns(_players.Object);
        _combatEvaluator.Setup(m => m.Database)
            .Returns(_database.Object);
        _database.Setup(m => m.Moves.GetMoveById(It.IsAny<int>()))
            .Returns(ScriptableObject.CreateInstance<Move>());
        _players.Setup(m => m.GetByClientId(myId))
            .Returns(_playerCharacter.Object);

        _effect3.TurnSpecialWasLastTriggeredOn = -10;
        _effect3.DoSpecial(didWinTurn);

        Assert.AreEqual(3, callArgs.Count);

        Assert.AreEqual(turn, callArgs[0].Round);
        callArgs[0].Execute(_combatEvaluator.Object);
        Assert.AreEqual(100, _playerCharacter.Object.SpecialMeter);
        Assert.IsInstanceOf<Character3Effect.FreeSpecialUse>(callArgs[0]);

        Assert.AreEqual(turn + 1, callArgs[1].Round);
        callArgs[1].Execute(_combatEvaluator.Object);
        Assert.AreEqual(100, _playerCharacter.Object.SpecialMeter);
        Assert.IsInstanceOf<Character3Effect.FreeSpecialUse>(callArgs[1]);

        Assert.AreEqual(turn + 2, callArgs[2].Round);
        callArgs[2].Execute(_combatEvaluator.Object);
        Assert.AreEqual(0, _playerCharacter.Object.SpecialMeter);
        Assert.IsInstanceOf<Character3Effect.ClearFreeSpecialUse>(callArgs[2]);
    }

    [Test]
    public void Character3EffectWontFireOnBackToBackTurns()
    {
        ulong myId = 0;
        int calls = 0;
        int turn = 0;
        _combatEvaluator.Setup(m => m.TurnNumber)
            .Returns(turn);
        _combatEvaluator.Setup(m => m.AddCombatCommand(It.IsAny<CombatCommandBase>()))
            .Callback(() => calls++);
        _playerCharacter.SetupProperty(m => m.ClientId, myId);
        _effect3.TurnSpecialWasLastTriggeredOn = -10;

        _effect3.DoSpecial(true);
        Assert.AreEqual(3, calls);

        _effect3.DoSpecial(true);
        Assert.AreEqual(3, calls);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        GameObject.Destroy(_testObject);
    }
}
