using System.Linq;
using Unity.Collections;

public class Turn
{
    public int TurnNumber;
    public bool Player1Wins;
    public bool Player2Wins;
    public bool IsDraw;
    public float Player1DamageTaken;
    public int Player1PositionChange;
    public float Player1SpecialGain;
    public float Player2DamageTaken;
    public int Player2PositionChange;
    public float Player2SpecialGain;
    public Move Player1LastMove;
    public Move Player1Move;
    public Move Player2LastMove;
    public Move Player2Move;
    public ComboType Player1ComboType;
    public ComboType Player2ComboType;
    public IPlayerCharacter Player1;
    public IPlayerCharacter Player2;

    private readonly TurnFactory _context;

    public Turn(TurnFactory context)
    {
        _context = context;
    }

    public Turn Initialize()
    {
        TurnNumber = _context.TurnHistory.GetCurrentTurnNumber();
        Player1 = _context.Players.GetByPlayerNumber(1);
        Player2 = _context.Players.GetByPlayerNumber(2);
        Player1Move = _context.Database.Moves.GetMoveById(Player1.Action);
        Player2Move = _context.Database.Moves.GetMoveById(Player2.Action);
        if (TurnNumber > 1)
        {
            Player1LastMove = _context.Database.Moves.GetMoveById(_context.TurnHistory.TurnDataList[^1].PlayerData1.Action);
            Player2LastMove = _context.Database.Moves.GetMoveById(_context.TurnHistory.TurnDataList[^1].PlayerData2.Action);
        }
        Player1DamageTaken = 0;
        Player2DamageTaken = 0;
        Player1PositionChange = 0;
        Player2PositionChange = 0;
        Player1SpecialGain = 0;
        Player2SpecialGain = 0;
        return this;
    }

    public Turn DetermineWinner()
    {
        Player1Wins = (Player2Move == null) || 
            (Player1Move != null && 
            _context.Database.MoveInteractions.DefeatedByType(Player1Move.MoveType).Contains(Player2Move.MoveType));
        Player2Wins = (Player1Move == null) || 
            (Player2Move != null && 
            _context.Database.MoveInteractions.DefeatedByType(Player2Move.MoveType).Contains(Player1Move.MoveType));
        IsDraw = Player1Wins == Player2Wins;
        return this;
    }

    public Turn DetermineComboStatus()
    {
        if (ShouldDetermineComboForPlayer(1))
        {
            Player1ComboType = DetermineComboForPlayer(1);
        }
        if (ShouldDetermineComboForPlayer(2))
        {
            Player2ComboType = DetermineComboForPlayer(2);
        }
        return this;
    }

    public Turn CheckForSpecialMovesAndExecute()
    {
        if (ShouldExecuteSpecialForPlayer(1))
        {
            ExecuteSpecialForPlayer(1);
        }
        if (ShouldExecuteSpecialForPlayer(2))
        {
            ExecuteSpecialForPlayer(2);
        }
        return this;
    }

    public Turn CalculateStateChanges()
    {
        CalculatePositionChanges();
        CalculateDamageTaken();
        CalculateSpecialMeterGained();
        return this;
    }


    public Turn ApplyStateChanges()
    {
        SetComboFreshness();
        AdjustComboCounts();
        Player1.DecreaseHealth(Player1DamageTaken);
        Player2.DecreaseHealth(Player2DamageTaken);
        Player1.IncreaseSpecialMeter(Player1SpecialGain);
        Player2.IncreaseSpecialMeter(Player2SpecialGain);
        Player1.IncreasePosition(Player1PositionChange);
        Player2.IncreasePosition(Player2PositionChange);
        return this;
    }

    public Turn ExecuteCombatCommands()
    {
        _context.CombatEvaluator.ExecuteCombatCommands();
        return this;
    }

    public Turn CheckAndSetSpecialUsability()
    {
        if (Player1.SpecialMeter >= 100f)
        {
            Player1.UsableMoveSet.EnableMoveByType(Move.Type.Special);
        }
        else
        {
            Player1.UsableMoveSet.DisableMoveByType(Move.Type.Special);
        }

        if (Player2.SpecialMeter >= 100f)
        {
            Player2.UsableMoveSet.EnableMoveByType(Move.Type.Special);
        }
        else
        {
            Player2.UsableMoveSet.DisableMoveByType(Move.Type.Special);
        }
        return this;
    }

    public TurnResult GetTurnData()
    {
        return new TurnResult
        {
            TurnNumber = TurnNumber,
            PlayerData1 = Player1.PlayerData,
            PlayerData2 = Player2.PlayerData,
            DamageToPlayer1 = Player1DamageTaken,
            DamageToPlayer2 = Player2DamageTaken,
            Summary = GetResultString()
        };
    }

    private void AdjustComboCounts()
    {
        if (IsDraw)
        {
            Player1.ResetComboCount();
            Player2.ResetComboCount();
        }
        else if (Player1Wins)
        {
            Player1.IncrementComboCount();
        }
        else if (Player2Wins)
        {
            Player2.IncrementComboCount();
        }
    }

    private void SetComboFreshness()
    {
        Player1.ComboIsFresh = 
            IsDraw || 
            Player2Wins || 
            Player1ComboType != ComboType.Combo;

        Player2.ComboIsFresh = 
            IsDraw ||
            Player1Wins ||
            Player2ComboType != ComboType.Combo;
    }

    private void CalculateSpecialMeterGained()
    {
        Player1SpecialGain = _context.Config.BaseSpecialGain;
        Player1SpecialGain *= (IsDraw || Player2Wins) ? _context.Config.SpecialGainOnLossModifier : 1f;

        Player2SpecialGain = _context.Config.BaseSpecialGain;
        Player2SpecialGain *= (IsDraw || Player1Wins) ?_context.Config.SpecialGainOnLossModifier : 1f;
    }

    private void CalculateDamageTaken()
    {
        if (IsDraw)
        {
            Player1DamageTaken = _context.Config.BaseDamage * _context.Config.ChipDamageModifier;
            Player2DamageTaken = _context.Config.BaseDamage * _context.Config.ChipDamageModifier;
        }
        else if (Player1Wins)
        {
            Player1DamageTaken = 0;
            Player2DamageTaken = _context.Config.BaseDamage;
            Player2DamageTaken *= CalculateComboDamageModifier(Player1ComboType);
        }
        else if (Player2Wins)
        {
            Player1DamageTaken = _context.Config.BaseDamage;
            Player1DamageTaken *= CalculateComboDamageModifier(Player2ComboType);
            Player2DamageTaken = 0;
        }
    }

    private void CalculatePositionChanges()
    {
        if (IsDraw)
        {
            Player1PositionChange = 0;
            Player2PositionChange = 0;
        }
        else if (Player1Wins)
        {
            var movementAmount = (Player1.Position < -1) ? 2 : 1;
            Player1PositionChange = movementAmount;
            Player2PositionChange = -1 * movementAmount;
        }
        else if (Player2Wins)
        {
            var movementAmount = (Player2.Position < -1) ? 2 : 1;
            Player1PositionChange = -1 * movementAmount;
            Player2PositionChange = movementAmount;
        }
    }

    private float CalculateComboDamageModifier(ComboType type)
    {
        return type switch
        {
            ComboType.Combo => _context.Config.ComboDamageMultiplier,
            ComboType.MixUp => _context.Config.ComboDamageMultiplier,
            ComboType.Special => _context.Config.ComboDamageMultiplier,
            _ => 1.0f
        };
    }

    private void ExecuteSpecialForPlayer(int playerNumber)
    {
        var player = (playerNumber == 1) ? Player1 : Player2;
        var didWin = (playerNumber == 1) ? Player1Wins : Player2Wins;
        player.Effect.DoSpecial(didWin && !IsDraw);
        player.SpecialMeter = 0;
        ClearSpecialGainForPlayer(playerNumber);
    }

    private void ClearSpecialGainForPlayer(int playerNumber)
    {
        if (playerNumber == 1)
        {
            Player1SpecialGain = 0;
        }
        else
        {
            Player2SpecialGain = 0;
        }
    }

    private bool ShouldExecuteSpecialForPlayer(int playerNumber)
    {
        var move = (playerNumber == 1) ? Player1Move : Player2Move;
        if (move == null)
        {
            return false;
        }
        return move.MoveType == Move.Type.Special;
    }

    private ComboType DetermineComboForPlayer(int playerNumber)
    {
        var lastMove = (playerNumber == 1) ? Player1LastMove : Player2LastMove;
        if (lastMove == null)
        {
            return ComboType.None;
        }

        if (lastMove.MoveType == Move.Type.Special)
        {
            return ComboType.Special;
        }

        var player = (playerNumber == 1) ? Player1 : Player2;
        var currentMove = (playerNumber == 1) ? Player1Move : Player2Move;
        ComboType comboType = DetermineComboType(player, lastMove, currentMove);
        return comboType;
    }

    private ComboType DetermineComboType(IPlayerCharacter player, Move lastMove, Move currentMove)
    {
        ComboType comboType = ComboType.Normal;
        if (player.Character.ComboPathSet.TryGetValue(lastMove.MoveType, out var comboPath))
        {
            if (player.ComboIsFresh)
            {
                if (comboPath.FreshComboMove == currentMove.MoveType)
                {
                    comboType = ComboType.Combo;
                }
                else if (comboPath.FreshMixUp.Contains(currentMove.MoveType))
                {
                    comboType = ComboType.MixUp;
                }
            }
            else
            {
                if (comboPath.ComboMove == currentMove.MoveType)
                {
                    comboType = ComboType.Combo;
                }
                else if (comboPath.MixUp.Contains<Move.Type>(currentMove.MoveType))
                {
                    comboType = ComboType.MixUp;

                }
            }
        }
        return comboType;
    }

    private bool ShouldDetermineComboForPlayer(int playerNumber)
    {
        var wins = (playerNumber == 1) ? Player1Wins : Player2Wins;
        var isInCombo = (playerNumber == 1) ? Player1.ComboCount > 0 : Player2.ComboCount > 0;
        var submittedAMove = (playerNumber == 1) ? Player1Move != null : Player2Move != null;
        var isNotFirstTurn = TurnNumber > 0;
        return 
            !IsDraw &&
            wins &&
            isInCombo &&
            submittedAMove &&
            isNotFirstTurn; 
    }

    private FixedString32Bytes GetResultString()
    {
        FixedString32Bytes result = "";
        if (IsDraw)
        {
            result = "Draw!";
        }
        else if (Player1Wins)
        {
            result = "Player 1 Wins";
        }
        else if (Player2Wins)
        {
            result = "Player 2 Wins";
        }
        return result;
    }
}
