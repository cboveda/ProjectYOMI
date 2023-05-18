using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using Zenject;

public class CombatEvaluator
{
    //Combat evaluation -- to be replaced
    [SerializeField] private float _baseDamage = 10f;
    [SerializeField] private float _baseSpecialGain = 25f;
    [SerializeField] private float _chipDamageModifier = 0.5f;
    [SerializeField] private float _specialGainOnLossModifier = 0.35f;
    [SerializeField] private float _comboDamageMultiplier = 1.5f;

    private IDatabase _database;
    private IPlayerDataCollection _players;
    private ITurnHistory _turnHistory;
    private readonly List<CombatCommandBase> _combatCommands;
    private int _turnNumber = 0;

    public virtual int TurnNumber { get => _turnNumber; }
    public virtual IPlayerDataCollection Players { get => _players; }
    public virtual IDatabase Database {  get => _database; }

    public CombatEvaluator()
    {
        _combatCommands = new();
    }

    [Inject]
    public void Construct(IDatabase database, IPlayerDataCollection playerDataCollection, ITurnHistory turnHistory)
    {
        _database = database;
        _players = playerDataCollection;
        _turnHistory = turnHistory;
    }

    public virtual void AddCombatCommand(CombatCommandBase combatCommand)
    {
        _combatCommands.Add(combatCommand);
    }

    public TurnData EvaluateTurnCombat()
    {
        _turnNumber += 1;

        var playerCharacter1 = _players.GetByPlayerNumber(1);
        var playerCharacter2 = _players.GetByPlayerNumber(2);

        if (playerCharacter1 == null || playerCharacter2 == null)
        {
            throw new Exception("Failed to get playerCharacter objects");
        }

        // Preliminary damage and special calculations
        var damageToPlayer1 = 0f;
        var damageToPlayer2 = 0f;
        var positionChangePlayer1 = 0;
        var positionChangePlayer2 = 0;
        var special1 = _baseSpecialGain;
        var special2 = _baseSpecialGain;

        // Read Inputs
        var action1 = playerCharacter1.PlayerData.Action;
        var action2 = playerCharacter2.PlayerData.Action;
        var movePlayer1 = _database.Moves.GetMoveById(action1);
        var movePlayer2 = _database.Moves.GetMoveById(action2);

        // Evaluate winner
        var player1Wins = (movePlayer2 == null) ||
            (movePlayer1 && _database.MoveInteractions.DefeatedByType(movePlayer1.MoveType).Contains(movePlayer2.MoveType));
        var player2Wins = (movePlayer1 == null) ||
            (movePlayer2 && _database.MoveInteractions.DefeatedByType(movePlayer2.MoveType).Contains(movePlayer1.MoveType));

        // Check combo context
        bool player1IsInCombo = playerCharacter1.ComboCount > 0;
        bool player2IsInCombo = playerCharacter2.ComboCount > 0;
        ComboType player1ComboType = ComboType.Normal;
        ComboType player2ComboType = ComboType.Normal;

        if (player1IsInCombo && TurnNumber > 0)
        {
            var lastMovePlayer1 = _database.Moves.GetMoveById(_turnHistory.TurnDataList[^1].PlayerData1.Action);
            if (playerCharacter1.Character.ComboPathSet.TryGetValue(lastMovePlayer1.MoveType, out var player1ComboPath))
            {

                if (playerCharacter1.ComboIsFresh)
                {
                    if (player1ComboPath.FreshComboMove == movePlayer1.MoveType)
                    {
                        player1ComboType = ComboType.Combo;
                        playerCharacter1.ComboIsFresh = false;
                    }
                    else if (player1ComboPath.FreshMixUp.Contains<Move.Type>(movePlayer1.MoveType))
                    {
                        player1ComboType = ComboType.MixUp;
                        playerCharacter1.ComboIsFresh = true;
                    }
                    else
                    {
                        playerCharacter1.ComboIsFresh = true;
                    }
                }
                else
                {
                    if (player1ComboPath.ComboMove == movePlayer1.MoveType)
                    {
                        player1ComboType = ComboType.Combo;
                        playerCharacter2.ComboIsFresh = false;
                    }
                    else if (player1ComboPath.MixUp.Contains<Move.Type>(movePlayer1.MoveType))
                    {
                        player1ComboType = ComboType.MixUp;
                        playerCharacter2.ComboIsFresh = true;
                    }
                    else
                    {
                        playerCharacter2.ComboIsFresh = true;
                    }
                }
            }
        }

        if (player2IsInCombo && TurnNumber > 0)
        {
            var lastMovePlayer2 = (TurnNumber > 0) ? _database.Moves.GetMoveById(_turnHistory.TurnDataList[^1].PlayerData2.Action) : null;
            if (playerCharacter2.Character.ComboPathSet.TryGetValue(lastMovePlayer2.MoveType, out var player2ComboPath))
            {

                if (playerCharacter2.ComboIsFresh)
                {
                    if (player2ComboPath.FreshComboMove == movePlayer2.MoveType)
                    {
                        player2ComboType = ComboType.Combo;
                    }
                    else if (player2ComboPath.FreshMixUp.Contains<Move.Type>(movePlayer2.MoveType))
                    {
                        player2ComboType = ComboType.MixUp;
                    }
                }
                else
                {
                    if (player2ComboPath.ComboMove == movePlayer2.MoveType)
                    {
                        player2ComboType = ComboType.Combo;
                    }
                    else if (player2ComboPath.MixUp.Contains<Move.Type>(movePlayer2.MoveType))
                    {
                        player2ComboType = ComboType.MixUp;
                    }
                }
            }
        }

        // Check for specials   
        if ((movePlayer1 != null) && (movePlayer1.MoveType == Move.Type.Special))
        {
            playerCharacter1.Effect.DoSpecial(player1Wins && !player2Wins);
            playerCharacter1.SpecialMeter = 0f;
            special1 = 0;
            playerCharacter1.ComboIsFresh = true;
        }

        if ((movePlayer2 != null) && (movePlayer2.MoveType == Move.Type.Special))
        {
            playerCharacter2.Effect.DoSpecial(!player1Wins && player2Wins);
            playerCharacter2.SpecialMeter = 0f;
            special2 = 0;
            playerCharacter1.ComboIsFresh = true;
        }

        // Check who won and apply updates
        if (player1Wins && !player2Wins)
        {
            positionChangePlayer1 = 1;
            positionChangePlayer2 = -1;
            damageToPlayer2 = _baseDamage;
            damageToPlayer2 *= player1ComboType switch
            {
                ComboType.Combo => _comboDamageMultiplier,
                ComboType.MixUp => _comboDamageMultiplier,
                _ => 1.0f
            };
            special2 *= _specialGainOnLossModifier;
            playerCharacter1.IncrementComboCount();
            playerCharacter2.ResetComboCount();
        }
        else if (!player1Wins && player2Wins)
        {
            positionChangePlayer1 = -1;
            positionChangePlayer2 = 1;
            damageToPlayer1 = _baseDamage;
            damageToPlayer1 *= player2ComboType switch
             {
                 ComboType.Combo => _comboDamageMultiplier,
                 ComboType.MixUp => _comboDamageMultiplier,
                 _ => 1.0f
             };
            special1 *= _specialGainOnLossModifier;
            playerCharacter1.ResetComboCount();
            playerCharacter1.IncrementComboCount();
        }
        else
        {
            damageToPlayer1 *= _chipDamageModifier;
            damageToPlayer2 *= _chipDamageModifier;
            special1 *= _specialGainOnLossModifier;
            special2 *= _specialGainOnLossModifier;
        }

        playerCharacter1.DecreaseHealth(damageToPlayer1);
        playerCharacter2.DecreaseHealth(damageToPlayer2);
        playerCharacter1.IncreaseSpecialMeter(special1);
        playerCharacter2.IncreaseSpecialMeter(special2);
        playerCharacter1.Position += positionChangePlayer1;
        playerCharacter2.Position += positionChangePlayer2;

        // Execute queued commands
        foreach (CombatCommandBase command in _combatCommands.Where(c => c.Round == _turnNumber))
        {
            command.Execute(this);
        }

        // Check if specials should be enabled
        if (playerCharacter1.SpecialMeter >= 100f)
        {
            playerCharacter1.UsableMoveSet.EnableMoveByType(Move.Type.Special);
        } 
        else
        {
            playerCharacter1.UsableMoveSet.DisableMoveByType(Move.Type.Special);
        }

        if (playerCharacter2.SpecialMeter >= 100f)
        {
            playerCharacter2.UsableMoveSet.EnableMoveByType(Move.Type.Special);
        }
        else
        {
            playerCharacter2.UsableMoveSet.DisableMoveByType(Move.Type.Special);
        }


        // Update combo context
        // TODO

        // Log combat
        var turnData = new TurnData
        {
            TurnNumber = _turnNumber,
            PlayerData1 = playerCharacter1.PlayerData,
            PlayerData2 = playerCharacter2.PlayerData,
            DamageToPlayer1 = damageToPlayer1,
            DamageToPlayer2 = damageToPlayer2,
            Summary = GetResultString(player1Wins, player2Wins)
        };

        // Reset moves
        playerCharacter1.ResetAction();
        playerCharacter2.ResetAction();
        
        // Temporary
        playerCharacter1.PlayerMovementController.UpdatePosition();
        playerCharacter2.PlayerMovementController.UpdatePosition();

        return turnData;
    }

    private FixedString32Bytes GetResultString(bool player1Win, bool player2Win)
    {
        FixedString32Bytes result = "";
        if (player1Win && player2Win)
        {
            result = "Draw!";
        }
        else if (player1Win)
        {
            result = "Player 1 Wins";
        }
        else if (player2Win)
        {
            result = "Player 2 Wins";
        }
        return result;
    }
}
