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
    public Move.Type Player1NextCombo;
    public Move.Type Player2NextCombo;
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
        if (_context.TurnHistory.GetLastTurn(out var lastTurn))
        {
            Player1LastMove = _context.Database.Moves.GetMoveById(lastTurn.PlayerData1.Action);
            Player2LastMove = _context.Database.Moves.GetMoveById(lastTurn.PlayerData2.Action);
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

    public Turn CalculateStateChanges()
    {
        CalculatePositionChanges();
        CalculateDamageTaken();
        CalculateSpecialMeterGained();
        return this;
    }


    public Turn ApplyStateChanges()
    {
        AdjustComboCounts();
        Player1.DecreaseHealth(Player1DamageTaken);
        Player2.DecreaseHealth(Player2DamageTaken);
        Player1.IncreaseSpecialMeter(Player1SpecialGain);
        Player2.IncreaseSpecialMeter(Player2SpecialGain);
        Player1.IncreasePosition(Player1PositionChange);
        Player2.IncreasePosition(Player2PositionChange);
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
            Player2.ResetComboCount();
        }
        else if (Player2Wins)
        {
            Player1.ResetComboCount();
            Player2.IncrementComboCount();
        }
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
        }
        else if (Player2Wins)
        {
            Player1DamageTaken = _context.Config.BaseDamage;
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
