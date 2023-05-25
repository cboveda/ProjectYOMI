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

            public class WhenLastMoveWasSpecialOrNull : ForPlayer1
            {
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

            public class WhenLastMoveWasNotSpecialOrNull : ForPlayer1
            {
                [SetUp]
                public new void SetUp()
                {
                    base.SetUp();

                    Character character = Resources.Load<Character>("Tests/101_TestCharacter");
                    foreach (Move.Type type in Enum.GetValues(typeof(Move.Type)))
                    {
                        if (character.ComboPathSet.TryGetValue(type, out var comboPath))
                        {
                            comboPath.Construct(_databaseMock.Object);
                        }
                    }
                    _player1Mock.Setup(m => m.Character).Returns(character);
                }

                [Test]
                [TestCase(Move.Type.LightAttack, Move.Type.HeavyAttack, true, ComboType.Combo)] //fresh combo
                [TestCase(Move.Type.LightAttack, Move.Type.Parry, true, ComboType.MixUp)] //fresh mixup
                [TestCase(Move.Type.Grab, Move.Type.HeavyAttack, false, ComboType.Combo)] //not fresh combo
                [TestCase(Move.Type.Grab, Move.Type.Parry, false, ComboType.MixUp)] //not fresh mixup
                [TestCase(Move.Type.Grab, Move.Type.Parry, true, ComboType.Normal)] //fresh normal
                [TestCase(Move.Type.Grab, Move.Type.Grab, false, ComboType.Normal)] //not fresh normal
                public void ForGivenLastMoveAndCurrentMoveAndFreshness(Move.Type lastMove, Move.Type currentMove, bool isFresh, ComboType expected)
                {
                    _player1Mock.Setup(m => m.ComboIsFresh).Returns(isFresh);
                    _turn.Player1Move = GetTestMoveByType(currentMove);
                    _turn.Player1LastMove = GetTestMoveByType(lastMove);
                    _turn.DetermineComboStatus();
                    Assert.AreEqual(expected, _turn.Player1ComboType);
                }

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

            public class WhenLastMoveWasSpecialOrNull : ForPlayer2
            {
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

            public class WhenLastMoveWasNotSpecialOrNull : ForPlayer2
            {
                [SetUp]
                public new void SetUp()
                {
                    base.SetUp();

                    Character character = Resources.Load<Character>("Tests/101_TestCharacter");
                    foreach (Move.Type type in Enum.GetValues(typeof(Move.Type)))
                    {
                        if (character.ComboPathSet.TryGetValue(type, out var comboPath))
                        {
                            comboPath.Construct(_databaseMock.Object);
                        }
                    }
                    _player2Mock.Setup(m => m.Character).Returns(character);
                }

                [Test]
                [TestCase(Move.Type.LightAttack, Move.Type.HeavyAttack, true, ComboType.Combo)] //fresh combo
                [TestCase(Move.Type.LightAttack, Move.Type.Parry, true, ComboType.MixUp)] //fresh mixup
                [TestCase(Move.Type.Grab, Move.Type.HeavyAttack, false, ComboType.Combo)] //not fresh combo
                [TestCase(Move.Type.Grab, Move.Type.Parry, false, ComboType.MixUp)] //not fresh mixup
                [TestCase(Move.Type.Grab, Move.Type.Parry, true, ComboType.Normal)] //fresh normal
                [TestCase(Move.Type.Grab, Move.Type.Grab, false, ComboType.Normal)] //not fresh normal
                public void ForGivenLastMoveAndCurrentMoveAndFreshness(Move.Type lastMove, Move.Type currentMove, bool isFresh, ComboType expected)
                {
                    _player2Mock.Setup(m => m.ComboIsFresh).Returns(isFresh);
                    _turn.Player2Move = GetTestMoveByType(currentMove);
                    _turn.Player2LastMove = GetTestMoveByType(lastMove);
                    _turn.DetermineComboStatus();
                    Assert.AreEqual(expected, _turn.Player2ComboType);
                }
            }
        }
    }

    public class ChecksForSpecialMovesAndExecutesCorrectly : TurnTest
    {
        private int _calls = 0;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();

            _turn.Player1 = _player1Mock.Object;
            _turn.Player2 = _player2Mock.Object;
            var effectMock = new Mock<ICharacterBaseEffect>();
            effectMock.Setup(m => m.DoSpecial(It.IsAny<bool>())).Callback(() => _calls++);
            _player1Mock.Setup(m => m.Effect).Returns(effectMock.Object);
            _player2Mock.Setup(m => m.Effect).Returns(effectMock.Object);
        }

        [Test]
        public void WhenPlayer1WonTurnAndUsedSpecial()
        {
            _turn.Player1Wins = true;
            _turn.IsDraw = false;
            _turn.Player1Move = _specialMove;
            _turn.Player2Move = _lightMove;

            var initial = _calls;
            _turn.CheckForSpecialMovesAndExecute();
            Assert.AreEqual(initial + 1, _calls);
        }

        [Test]
        public void WhenPlayer1WonTurnAndDidNotUseSpecial()
        {
            _turn.Player1Wins = true;
            _turn.IsDraw = false;
            _turn.Player1Move = _parryMove;
            _turn.Player2Move = _lightMove;

            var initial = _calls;
            _turn.CheckForSpecialMovesAndExecute();
            Assert.AreEqual(initial, _calls);
        }

        [Test]
        public void WhenPlayerDidNotSelectAMove()
        {
            _turn.Player1Move = null;
            _turn.Player2Move = null;

            var initial = _calls;
            _turn.CheckForSpecialMovesAndExecute();
            Assert.AreEqual(initial, _calls);
        }

        [Test]
        public void WhenPlayer2WonTurnAndUsedSpecial()
        {
            _turn.Player2Wins = true;
            _turn.IsDraw = false;
            _turn.Player2Move = _specialMove;
            _turn.Player1Move = _lightMove;

            var initial = _calls;
            _turn.CheckForSpecialMovesAndExecute();
            Assert.AreEqual(initial + 1, _calls);
        }

        [Test]
        public void WhenPlayer2WonTurnAndDidNotUseSpecial()
        {
            _turn.Player2Wins = true;
            _turn.IsDraw = false;
            _turn.Player2Move = _parryMove;
            _turn.Player1Move = _lightMove;

            var initial = _calls;
            _turn.CheckForSpecialMovesAndExecute();
            Assert.AreEqual(initial, _calls);
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
            [TestCase(ComboType.Combo)]
            [TestCase(ComboType.Normal)]
            [TestCase(ComboType.Special)]
            [TestCase(ComboType.None)]
            [TestCase(ComboType.MixUp)]
            public void AndPlayer1ComboTypeIs(ComboType type)
            {
                var expectedMultiplier = DetermineExpectedMultiplier(type);
                var expectedOutgoingDamage = _configuration.BaseDamage * expectedMultiplier;

                _turn.Player1ComboType = type;
                _turn.CalculateStateChanges();
                Assert.AreEqual(0, _turn.Player1DamageTaken);
                Assert.AreEqual(expectedOutgoingDamage, _turn.Player2DamageTaken);
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
            [TestCase(ComboType.Combo)]
            [TestCase(ComboType.Normal)]
            [TestCase(ComboType.Special)]
            [TestCase(ComboType.None)]
            [TestCase(ComboType.MixUp)]
            public void AndPlayer2ComboTypeIs(ComboType type)
            {
                var expectedMultiplier = DetermineExpectedMultiplier(type);
                var expectedOutgoingDamage = _configuration.BaseDamage * expectedMultiplier;

                _turn.Player2ComboType = type;
                _turn.CalculateStateChanges();
                Assert.AreEqual(0, _turn.Player2DamageTaken);
                Assert.AreEqual(expectedOutgoingDamage, _turn.Player1DamageTaken);
            }

            [Test]
            public void CheckSpecialGain()
            {
                _turn.CalculateStateChanges();
                Assert.AreEqual(_configuration.BaseSpecialGain * _configuration.SpecialGainOnLossModifier, _turn.Player1SpecialGain);
                Assert.AreEqual(_configuration.BaseSpecialGain, _turn.Player2SpecialGain);
            }
        }

        private float DetermineExpectedMultiplier(ComboType type)
        {
            return (type == ComboType.Combo ||
                 type == ComboType.MixUp ||
                 type == ComboType.Special) ?
                    _configuration.ComboDamageMultiplier :
                    1.0f;
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

        public class ForComboFreshness : AppliesStateChangesCorrectly
        {
            [SetUp]
            public new void SetUp()
            {
                base.SetUp();
                _player1Mock.SetupProperty(m => m.ComboIsFresh, false);
                _player2Mock.SetupProperty(m => m.ComboIsFresh, false);
            }

            [Test]
            public void WhenGameIsADraw()
            {
                _turn.IsDraw = true;
                _turn.ApplyStateChanges();
                Assert.IsTrue(_turn.Player1.ComboIsFresh);
                Assert.IsTrue(_turn.Player2.ComboIsFresh);
            }

            [Test]
            public void WhenPlayer1WinsAndIsNotInExtendedCombo()
            {
                _turn.IsDraw = false;
                _turn.Player1Wins = true;
                _turn.Player1ComboType = ComboType.Normal;
                _turn.ApplyStateChanges();
                Assert.IsTrue(_turn.Player1.ComboIsFresh);
            } 
            
            [Test]
            public void WhenPlayer2WinsAndIsNotInExtendedCombo()
            {
                _turn.IsDraw = false;
                _turn.Player2Wins = true;
                _turn.Player2ComboType = ComboType.Normal;
                _turn.ApplyStateChanges();
                Assert.IsTrue(_turn.Player2.ComboIsFresh);
            }
            
            [Test]
            public void WhenPlayer1WinsButIsInExtendedCombo()
            {
                _turn.IsDraw = false;
                _turn.Player1Wins = true;
                _turn.Player1ComboType = ComboType.Combo;
                _turn.ApplyStateChanges();
                Assert.IsFalse(_turn.Player1.ComboIsFresh);
            }
            
            [Test]
            public void WhenPlayer2WinsButIsInExtendedCombo()
            {
                _turn.IsDraw = false;
                _turn.Player2Wins = true;
                _turn.Player2ComboType = ComboType.Combo;
                _turn.ApplyStateChanges();
                Assert.IsFalse(_turn.Player2.ComboIsFresh);
            }

            [Test]
            public void WhenPlayer1DoesNotWin()
            {
                _turn.IsDraw = false;
                _turn.Player1Wins = false;
                _turn.ApplyStateChanges();
                Assert.IsTrue(_turn.Player1.ComboIsFresh);
            }
            
            [Test]
            public void WhenPlayer2DoesNotWin()
            {
                _turn.IsDraw = false;
                _turn.Player2Wins = false;
                _turn.ApplyStateChanges();
                Assert.IsTrue(_turn.Player2.ComboIsFresh);
            }
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

    public class ExecutesCombatCommandsCorrectly : TurnTest
    {
        int _calls;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _calls = 0;
            _combatCommandsMock.Setup(m => m.ExecuteCombatCommands())
                .Callback(() => _calls++);
        }

        [Test]
        public void ByCallingCombatCommandExecutor()
        {
            _turn.ExecuteCombatCommands();
            Assert.AreEqual(1, _calls);
        }
    }

    public class ChecksAndSetsSpecialUsabilityCorrectly : TurnTest 
    {
        bool _player1SpecialEnabled;
        bool _player2SpecialEnabled;

        [SetUp] 
        public new void SetUp()
        {
            base.SetUp();

            _turn.Player1 = _player1Mock.Object;
            _turn.Player2 = _player2Mock.Object;
            Mock<IUsableMoveSet> player1UsableMoveSet = new();
            Mock<IUsableMoveSet> player2UsableMoveSet = new();
            player1UsableMoveSet.Setup(m => m.EnableMoveByType(Move.Type.Special)).Callback(() => _player1SpecialEnabled = true);
            player2UsableMoveSet.Setup(m => m.EnableMoveByType(Move.Type.Special)).Callback(() => _player2SpecialEnabled = true);
            player1UsableMoveSet.Setup(m => m.DisableMoveByType(Move.Type.Special)).Callback(() => _player1SpecialEnabled = false);
            player2UsableMoveSet.Setup(m => m.DisableMoveByType(Move.Type.Special)).Callback(() => _player2SpecialEnabled= false);
            _player1Mock.Setup(m => m.UsableMoveSet).Returns(player1UsableMoveSet.Object);
            _player2Mock.Setup(m => m.UsableMoveSet).Returns(player2UsableMoveSet.Object);
        }

        [Test]
        [TestCase(101f, true)]
        [TestCase(100f, true)]
        [TestCase(99f, false)]
        [TestCase(0f, false)]
        public void ForAGivenSpecialValue(float value, bool expected)
        {
            _player1Mock.SetupProperty(m => m.SpecialMeter, value);
            _player2Mock.SetupProperty(m => m.SpecialMeter, value);
            _turn.CheckAndSetSpecialUsability();
            Assert.AreEqual(expected, _player1SpecialEnabled);
            Assert.AreEqual(expected, _player2SpecialEnabled);
        }
    }

    public class DeterminesNextComboMoveCorrectly : TurnTest
    {
        ComboPath _comboPath;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();

            _turn.Player1 = _player1Mock.Object;
            _turn.Player2 = _player2Mock.Object;

            _player1Mock.SetupProperty(m => m.ComboCount, 0);
            _player2Mock.SetupProperty(m => m.ComboCount, 0);
            _turn.Player1.ComboCount = 1;
            _turn.Player2.ComboCount = 1;

            _player1Mock.SetupProperty(m => m.ComboIsFresh, true);
            _player2Mock.SetupProperty(m => m.ComboIsFresh, true);

            Character character = Resources.Load<Character>("Tests/101_TestCharacter");
            foreach (Move.Type type in Enum.GetValues(typeof(Move.Type)))
            {
                if (character.ComboPathSet.TryGetValue(type, out var comboPath))
                {
                    comboPath.Construct(_databaseMock.Object);
                }
            }
            _player1Mock.Setup(m => m.Character).Returns(character);
            _player2Mock.Setup(m => m.Character).Returns(character);
            character.ComboPathSet.TryGetValue(Move.Type.LightAttack, out _comboPath);
            _turn.Player1Move = character.CharacterMoveSet.LightAttack;
            _turn.Player2Move = character.CharacterMoveSet.LightAttack;
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DeterminesNextComboMove(bool freshness)
        {
            _player1Mock.Object.ComboIsFresh = freshness;
            _player2Mock.Object.ComboIsFresh = freshness;
            Move.Type expected = (freshness) ? _comboPath.FreshComboMove : _comboPath.ComboMove;

            _turn.DetermineNextComboMove();
            Assert.AreEqual(expected, _turn.Player1NextCombo);
            Assert.AreEqual(expected, _turn.Player2NextCombo);
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


