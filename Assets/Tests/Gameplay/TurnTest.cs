using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.TestTools;

public class TurnTest
{
    protected Turn _turn;

    protected TurnFactory _factory;
    protected CombatConfiguration _configuration;
    protected Mock<IDatabase> _databaseMock;
    protected Mock<IPlayerDataCollection> _playersMock;
    protected Mock<ITurnHistory> _turnHistoryMock;
    protected Mock<CombatCommandExecutor> _combatCommandsMock;

    protected Mock<IPlayerCharacter> _player1Mock;
    protected Mock<IPlayerCharacter> _player2Mock;

    protected Move _lightMove;
    protected Move _heavyMove;
    protected Move _grabMove;
    protected Move _parryMove;
    protected Move _specialMove;
    protected Mock<IMoveDatabase> _moveDatabaseMock;
    protected Mock<IMoveInteractions> _moveInteractionsMock;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _configuration = ScriptableObject.CreateInstance<CombatConfiguration>();
        _databaseMock = new Mock<IDatabase>();
        _playersMock = new Mock<IPlayerDataCollection>();
        _turnHistoryMock = new Mock<ITurnHistory>();
        _combatCommandsMock = new Mock<CombatCommandExecutor>();

        _player1Mock = new Mock<IPlayerCharacter>();
        _player2Mock = new Mock<IPlayerCharacter>();
        _playersMock.Setup(m => m.GetByPlayerNumber(1)).Returns(_player1Mock.Object);
        _playersMock.Setup(m => m.GetByPlayerNumber(2)).Returns(_player2Mock.Object);

        _lightMove = ScriptableObject.CreateInstance<Move>();
        _heavyMove = ScriptableObject.CreateInstance<Move>();
        _grabMove = ScriptableObject.CreateInstance<Move>();
        _parryMove = ScriptableObject.CreateInstance<Move>();
        _specialMove = ScriptableObject.CreateInstance<Move>();
        _lightMove.MoveType = Move.Type.LightAttack;
        _heavyMove.MoveType= Move.Type.HeavyAttack;
        _grabMove.MoveType = Move.Type.Grab;
        _parryMove.MoveType = Move.Type.Parry;
        _specialMove.MoveType = Move.Type.Special;
        _lightMove.Id = 1;
        _heavyMove.Id = 2;
        _grabMove.Id = 3;
        _parryMove.Id = 4;
        _specialMove.Id = 5;

        _moveDatabaseMock = new Mock<IMoveDatabase>();
        _databaseMock.Setup(m => m.Moves).Returns(_moveDatabaseMock.Object);

        _moveDatabaseMock.Setup(m => m.GetMoveById(_lightMove.Id)).Returns(_lightMove);
        _moveDatabaseMock.Setup(m => m.GetMoveById(_heavyMove.Id)).Returns(_heavyMove);
        _moveDatabaseMock.Setup(m => m.GetMoveById(_grabMove.Id)).Returns(_grabMove);
        _moveDatabaseMock.Setup(m => m.GetMoveById(_parryMove.Id)).Returns(_parryMove);
        _moveDatabaseMock.Setup(m => m.GetMoveById(_specialMove.Id)).Returns(_specialMove);

        _moveInteractionsMock = new Mock<IMoveInteractions>();
        _databaseMock.Setup(m => m.MoveInteractions).Returns(_moveInteractionsMock.Object);
        _moveInteractionsMock.Setup(m => m.DefeatedByType(Move.Type.LightAttack))
            .Returns(new List<Move.Type>() { Move.Type.HeavyAttack, Move.Type.Grab });
        _moveInteractionsMock.Setup(m => m.DefeatedByType(Move.Type.HeavyAttack))
            .Returns(new List<Move.Type>() { Move.Type.Grab, Move.Type.Parry });
        _moveInteractionsMock.Setup(m => m.DefeatedByType(Move.Type.Parry))
            .Returns(new List<Move.Type>() { Move.Type.LightAttack, Move.Type.Special });
        _moveInteractionsMock.Setup(m => m.DefeatedByType(Move.Type.Grab))
            .Returns(new List<Move.Type>() { Move.Type.Parry, Move.Type.Special });
        _moveInteractionsMock.Setup(m => m.DefeatedByType(Move.Type.Special))
            .Returns(new List<Move.Type>() { Move.Type.LightAttack, Move.Type.HeavyAttack });

        _factory = new TurnFactory();
        _factory.Construct(
            config: _configuration,
            database: _databaseMock.Object,
            players: _playersMock.Object,
            history: _turnHistoryMock.Object,
            combatEvaluator: _combatCommandsMock.Object);
    }

    [SetUp]
    public void SetUp()
    {
        _turn = _factory.GetTurn();
    }

    public class InitializesCorrectly : TurnTest
    {
        [SetUp]
        public new void SetUp()
        {
            base.SetUp();

            _player1Mock.Setup(m => m.Action).Returns(_lightMove.Id);
            _player2Mock.Setup(m => m.Action).Returns(_heavyMove.Id);
            TurnResult turnResult = new();
            turnResult.PlayerData1.Action = _parryMove.Id;
            turnResult.PlayerData2.Action = _grabMove.Id;
            _turnHistoryMock.Setup(m => m.GetLastTurn(out turnResult)).Returns(true);
        }

        [Test]
        [TestCase (1)]
        [TestCase (2)]
        public void OnASpecificTurn(int turnNumber)
        {
            _turnHistoryMock.Setup(m => m.GetCurrentTurnNumber()).Returns(turnNumber);

            _turn.Initialize();
            Assert.AreEqual(turnNumber, _turn.TurnNumber);
            Assert.AreEqual(_player1Mock.Object, _turn.Player1);
            Assert.AreEqual(_player2Mock.Object, _turn.Player2);
            Assert.AreEqual(_lightMove, _turn.Player1Move);
            Assert.AreEqual(_heavyMove, _turn.Player2Move);
            Assert.AreEqual(0, _turn.Player1DamageTaken);
            Assert.AreEqual(0, _turn.Player2DamageTaken);
            Assert.AreEqual(0, _turn.Player1PositionChange);
            Assert.AreEqual(0, _turn.Player2PositionChange);
            Assert.AreEqual(0, _turn.Player1SpecialGain);
            Assert.AreEqual(0, _turn.Player2SpecialGain);
            if (turnNumber > 1)
            {
                Assert.AreEqual(_parryMove, _turn.Player1LastMove);
                Assert.AreEqual(_grabMove, _turn.Player2LastMove);
            }
        }
    }

    public class DeterminesWinnerCorrectly : TurnTest
    {
        [Test]
        [TestCase(Move.Type.LightAttack, Move.Type.HeavyAttack, ExpectedResult.Player1Wins)]
        [TestCase(Move.Type.LightAttack, Move.Type.Parry, ExpectedResult.Player2Wins)]
        [TestCase(Move.Type.LightAttack, Move.Type.LightAttack, ExpectedResult.Draw)]
        public void ForMoveSelections(Move.Type player1, Move.Type player2, ExpectedResult expectedResult)
        {
            bool expectedPlayer1Wins = (expectedResult == ExpectedResult.Player1Wins);
            bool expectedPlayer2Wins = (expectedResult == ExpectedResult.Player2Wins);
            bool expectedDraw = (expectedResult == ExpectedResult.Draw);

            _turn.Player1Move = GetTestMoveByType(player1);
            _turn.Player2Move = GetTestMoveByType(player2);

            _turn.DetermineWinner();
            Assert.AreEqual(expectedPlayer1Wins, _turn.Player1Wins);
            Assert.AreEqual(expectedPlayer2Wins, _turn.Player2Wins);
            Assert.AreEqual(expectedDraw, _turn.IsDraw);
        }

        public enum ExpectedResult
        {
            Player1Wins,
            Player2Wins,
            Draw
        }
    }

    public class DeterminesComboStatusCorrectly : TurnTest
    {
        public class ForPlayer1 : DeterminesComboStatusCorrectly
        {
            [SetUp]
            public new void SetUp()
            {
                base.SetUp();

                _turn.Player1Wins = true;
                _player1Mock.Setup(m => m.ComboCount).Returns(1);
                _turn.Player1Move = _lightMove;
                _turn.Player1 = _player1Mock.Object;

                _turn.Player2Wins = false;
                _player2Mock.Setup(m => m.ComboCount).Returns(0);
                _turn.Player2Move = _lightMove;
                _turn.Player2 = _player2Mock.Object;

                _turn.TurnNumber = 1;
                _turn.IsDraw = false;
            }

            [Test]
            public void WhenLastMoveWasNull()
            {
                _turn.Player1LastMove = null;
                _turn.DetermineComboStatus();
                Assert.AreEqual(ComboType.None, _turn.Player1ComboType);
            }

            [Test]
            public void WhenLastMoveWasSpecial()
            {
                _turn.Player1LastMove = _specialMove;
                _turn.DetermineComboStatus();
                Assert.AreEqual(ComboType.Special, _turn.Player1ComboType);
            }
        }

        public class ForPlayer2 : DeterminesComboStatusCorrectly
        {
            [SetUp]
            public new void SetUp()
            {
                base.SetUp();

                _turn.Player1Wins = false;
                _player1Mock.Setup(m => m.ComboCount).Returns(0);
                _turn.Player1Move = _lightMove;
                _turn.Player1 = _player1Mock.Object;

                _turn.Player2Wins = true;
                _player2Mock.Setup(m => m.ComboCount).Returns(1);
                _turn.Player2Move = _lightMove;
                _turn.Player2 = _player2Mock.Object;

                _turn.TurnNumber = 1;
                _turn.IsDraw = false;
            }

            [Test]
            public void WhenLastMoveWasNull()
            {
                _turn.Player2LastMove = null;
                _turn.DetermineComboStatus();
                Assert.AreEqual(ComboType.None, _turn.Player2ComboType);
            }

            [Test]
            public void WhenLastMoveWasSpecial()
            {
                _turn.Player2LastMove = _specialMove;
                _turn.DetermineComboStatus();
                Assert.AreEqual(ComboType.Special, _turn.Player2ComboType);
            }
        }
    }

    public Move GetTestMoveByType(Move.Type type)
    {
        return type switch
        {
            Move.Type.LightAttack => _lightMove,
            Move.Type.HeavyAttack => _heavyMove,
            Move.Type.Parry => _parryMove,
            Move.Type.Grab => _grabMove,
            Move.Type.Special => _specialMove,
            _ => null
        };
    }
}


