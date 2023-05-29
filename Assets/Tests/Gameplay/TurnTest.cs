using System;
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
        _heavyMove.MoveType = Move.Type.HeavyAttack;
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

        _moveInteractionsMock.Setup(m => m.DefeatsType(Move.Type.LightAttack))
            .Returns(new List<Move.Type>() { Move.Type.Parry, Move.Type.Special });
        _moveInteractionsMock.Setup(m => m.DefeatsType(Move.Type.HeavyAttack))
            .Returns(new List<Move.Type>() { Move.Type.LightAttack, Move.Type.Special });
        _moveInteractionsMock.Setup(m => m.DefeatsType(Move.Type.Parry))
            .Returns(new List<Move.Type>() { Move.Type.HeavyAttack, Move.Type.Grab });
        _moveInteractionsMock.Setup(m => m.DefeatsType(Move.Type.Grab))
            .Returns(new List<Move.Type>() { Move.Type.LightAttack, Move.Type.HeavyAttack });
        _moveInteractionsMock.Setup(m => m.DefeatsType(Move.Type.Special))
            .Returns(new List<Move.Type>() { Move.Type.Parry, Move.Type.Grab });

        _factory = new TurnFactory();
        _factory.Construct(
            config: _configuration,
            database: _databaseMock.Object,
            players: _playersMock.Object,
            history: _turnHistoryMock.Object);
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
        [TestCase(1)]
        [TestCase(2)]
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

    public class CalculatesStateChangesCorrectly : TurnTest
    {
        [SetUp]
        public new void SetUp()
        {
            base.SetUp();

            _turn.Player1 = _player1Mock.Object;
            _turn.Player2 = _player2Mock.Object;
        }

        public class WhenIsDraw : CalculatesStateChangesCorrectly
        {
            [Test]
            public void IsTrue()
            {
                _turn.IsDraw = true;
                _turn.CalculateStateChanges();
                var expectedPosition = 0;
                var expectedDamage = _configuration.BaseDamage * _configuration.ChipDamageModifier;
                var expectedSpecial = _configuration.BaseSpecialGain * _configuration.SpecialGainOnLossModifier;
                Assert.AreEqual(expectedPosition, _turn.Player1PositionChange);
                Assert.AreEqual(expectedPosition, _turn.Player2PositionChange);
                Assert.AreEqual(expectedDamage, _turn.Player1DamageTaken);
                Assert.AreEqual(expectedDamage, _turn.Player2DamageTaken);
                Assert.AreEqual(expectedSpecial, _turn.Player1SpecialGain);
                Assert.AreEqual(expectedSpecial, _turn.Player2SpecialGain);
            }
        }

        public class WhenPlayer1Wins : CalculatesStateChangesCorrectly
        {
            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
                _turn.Player1Wins = true;
                _turn.Player2Wins = false;
                _turn.IsDraw = false;
            }

            [Test]
            public void AndPlayer1PositionIsNegative()
            {
                int startPosition = -2;
                _player1Mock.SetupProperty<int>(m => m.Position, startPosition);
                _turn.CalculateStateChanges();
                Assert.AreEqual(2, _turn.Player1PositionChange);
            }

            [Test]
            public void AndPlayer1PositionIsPositive()
            {
                int startPosition = 1;
                _player1Mock.SetupProperty<int>(m => m.Position, startPosition);
                _turn.CalculateStateChanges();
                Assert.AreEqual(1, _turn.Player1PositionChange);
            }

            [Test]
            public void CheckSpecialGain()
            {
                _turn.CalculateStateChanges();
                Assert.AreEqual(_configuration.BaseSpecialGain, _turn.Player1SpecialGain);
                Assert.AreEqual(_configuration.BaseSpecialGain * _configuration.SpecialGainOnLossModifier, _turn.Player2SpecialGain);
            }
        }

        public class WhenPlayer2Wins : CalculatesStateChangesCorrectly
        {
            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
                _turn.Player1Wins = false;
                _turn.Player2Wins = true;
                _turn.IsDraw = false;
            }

            [Test]
            public void AndPlayer2PositionIsNegative()
            {
                int startPosition = -2;
                _player2Mock.SetupProperty<int>(m => m.Position, startPosition);
                _turn.CalculateStateChanges();
                Assert.AreEqual(2, _turn.Player2PositionChange);
            }

            [Test]
            public void AndPlayer2PositionIsPositive()
            {
                int startPosition = 1;
                _player2Mock.SetupProperty<int>(m => m.Position, startPosition);
                _turn.CalculateStateChanges();
                Assert.AreEqual(1, _turn.Player2PositionChange);
            }

            [Test]
            public void CheckSpecialGain()
            {
                _turn.CalculateStateChanges();
                Assert.AreEqual(_configuration.BaseSpecialGain * _configuration.SpecialGainOnLossModifier, _turn.Player1SpecialGain);
                Assert.AreEqual(_configuration.BaseSpecialGain, _turn.Player2SpecialGain);
            }
        }
    }

    public class AppliesStateChangesCorrectly : TurnTest
    {
        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _turn.Player1 = _player1Mock.Object;
            _turn.Player2 = _player2Mock.Object;
        }

        public class ForComboCounts : AppliesStateChangesCorrectly
        {
            int _player1Combo;
            int _player2Combo;

            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
                _player1Combo = 1;
                _player2Combo = 1;
                _player1Mock.Setup(m => m.ResetComboCount()).Callback(() => _player1Combo = 0);
                _player2Mock.Setup(m => m.ResetComboCount()).Callback(() => _player2Combo = 0);
                _player1Mock.Setup(m => m.IncrementComboCount()).Callback(() => _player1Combo++);
                _player2Mock.Setup(m => m.IncrementComboCount()).Callback(() => _player2Combo++);
            }

            [Test]
            public void WhenIsDraw()
            {
                _turn.IsDraw = true;
                _turn.Player1Wins = false;
                _turn.Player2Wins = false;
                _turn.ApplyStateChanges();
                Assert.AreEqual(0, _player1Combo);
                Assert.AreEqual(0, _player2Combo);
            }

            [Test]
            public void WhenPlayer1Wins()
            {
                _turn.IsDraw = false;
                _turn.Player1Wins = true;
                _turn.Player2Wins = false;
                var expected = _player1Combo + 1;
                _turn.ApplyStateChanges();
                Assert.AreEqual(expected, _player1Combo);
                Assert.AreEqual(0, _player2Combo);
            }
            
            [Test]
            public void WhenPlayer2Wins()
            {
                _turn.IsDraw = false;
                _turn.Player1Wins = false;
                _turn.Player2Wins = true;
                var expected = _player2Combo + 1;
                _turn.ApplyStateChanges();
                Assert.AreEqual(0, _player1Combo);
                Assert.AreEqual(expected, _player2Combo);
            }
        }

        public class ForPlayerDataValues : AppliesStateChangesCorrectly
        {
            float _player1Health;
            float _player2Health;
            float _player1Special;
            float _player2Special;
            int _player1Position;
            int _player2Position;


            [SetUp]
            public new void SetUp()
            {
                base.SetUp();

                _player1Health = 10f;
                _player2Health = 10f;
                _player1Special = 10f;
                _player2Special = 10f;
                _player1Position = 0;
                _player2Position = 0;

                _player1Mock.Setup(m => m.DecreaseHealth(It.IsAny<float>()))
                    .Callback<float>((value) => _player1Health -= value);
                _player2Mock.Setup(m => m.DecreaseHealth(It.IsAny<float>()))
                    .Callback<float>((value) => _player2Health -= value);
                _player1Mock.Setup(m => m.IncreaseSpecialMeter(It.IsAny<float>()))
                    .Callback<float>((value) => _player1Special += value);
                _player2Mock.Setup(m => m.IncreaseSpecialMeter(It.IsAny<float>()))
                    .Callback<float>((value) => _player2Special += value);
                _player1Mock.Setup(m => m.IncreasePosition(It.IsAny<int>()))
                    .Callback<int>((value) => _player1Position += value);
                _player2Mock.Setup(m => m.IncreasePosition(It.IsAny<int>()))
                    .Callback<int>((value) => _player2Position += value);
            }

            [Test]
            [TestCase(-1f)]
            [TestCase(0f)]
            [TestCase(1f)]
            public void WhenApplyingDamage(float value)
            {
                _turn.Player1DamageTaken = value;
                _turn.Player2DamageTaken = value;
                var expectedPlayer1 = _player1Health - value;
                var expectedPlayer2 = _player2Health - value;
                _turn.ApplyStateChanges();
                Assert.AreEqual(expectedPlayer1, _player1Health);
                Assert.AreEqual(expectedPlayer2, _player2Health);
            }
            
            [Test]
            [TestCase(-1f)]
            [TestCase(0f)]
            [TestCase(1f)]
            public void WhenApplyingSpecialGain(float value)
            {
                _turn.Player1SpecialGain = value;
                _turn.Player2SpecialGain = value;
                var expectedPlayer1 = _player1Special + value;
                var expectedPlayer2 = _player2Special + value;
                _turn.ApplyStateChanges();
                Assert.AreEqual(expectedPlayer1, _player1Special);
                Assert.AreEqual(expectedPlayer2, _player2Special);
            }

            [Test]
            [TestCase(-1)]
            [TestCase(0)]
            [TestCase(1)]
            public void WhenApplyingPositionChange(int value)
            {
                _turn.Player1PositionChange = value;
                _turn.Player2PositionChange = value;
                var expectedPlayer1 = _player1Position+ value;
                var expectedPlayer2 = _player2Position + value;
                _turn.ApplyStateChanges();
                Assert.AreEqual(expectedPlayer1, _player1Position);
                Assert.AreEqual(expectedPlayer2, _player2Position);
            }
        }
    }

    public class ProducesTurnResultData : TurnTest
    {
        [Test]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        public void WhenGetTurnDataIsCalled(bool isDraw, bool player1Wins, bool player2Wins)
        {
            int turnNumber = 1;
            float damageToPlayer1 = 10f;
            float damageToPlayer2 = 5f;
            PlayerData playerData1 = new PlayerData(99);
            PlayerData playerData2 = new PlayerData(98);  
            _player1Mock.Setup(m => m.PlayerData).Returns(playerData1);
            _player2Mock.Setup(m => m.PlayerData).Returns(playerData2);
            _turn.TurnNumber = turnNumber;
            _turn.Player1 = _player1Mock.Object;
            _turn.Player2 = _player2Mock.Object;
            _turn.Player1DamageTaken = damageToPlayer1;
            _turn.Player2DamageTaken = damageToPlayer2;
            _turn.IsDraw = isDraw;
            _turn.Player1Wins = player1Wins;
            _turn.Player2Wins = player2Wins;

            var turnData = _turn.GetTurnData();
            Assert.AreEqual(turnNumber, turnData.TurnNumber);
            Assert.AreEqual(playerData1, turnData.PlayerData1);
            Assert.AreEqual(playerData2, turnData.PlayerData2);
            Assert.AreEqual(damageToPlayer1, turnData.DamageToPlayer1);
            Assert.AreEqual(damageToPlayer2, turnData.DamageToPlayer2);
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


