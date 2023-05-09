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

    private IDatabase _database;
    private IPlayerDataCollection _players;
    private List<CombatCommandBase> _combatCommands;
    private int _turnNumber = 0;

    public int TurnNumber { get => _turnNumber; }

    public CombatEvaluator()
    {
        _combatCommands = new();
    }

    [Inject]
    public void Construct(IDatabase database, IPlayerDataCollection playerDataCollection)
    {
        _database = database;
        _players = playerDataCollection;
    }

    public void AddCombatCommand(CombatCommandBase combatCommand)
    {
        _combatCommands.Add(combatCommand);
    }

    public TurnData EvaluateTurnCombat()
    {
        _turnNumber += 1;

        var playerCharacter1 = _players.GetByPlayerNumber(1);
        var playerCharacter2 = _players.GetByPlayerNumber(2);

        if (!playerCharacter1 || !playerCharacter2)
        {
            throw new Exception("Failed to get playerCharacter objects");
        }

        // Read Inputs
        var action1 = playerCharacter1.PlayerData.Action;
        var action2 = playerCharacter2.PlayerData.Action;
        var movePlayer1 = _database.Moves.GetMoveById(action1);
        var movePlayer2 = _database.Moves.GetMoveById(action2);
        var player1Wins = (movePlayer2 == null) ||
            (movePlayer1 && movePlayer1.Defeats(movePlayer2.MoveType));
        var player2Wins = (movePlayer1 == null) ||
            (movePlayer2 && movePlayer2.Defeats(movePlayer1.MoveType));

        // Check for specials   
        if ((movePlayer1 != null) && (movePlayer1.MoveType == Move.Type.Special))
        {
            playerCharacter1.Effect.DoSpecial();
            playerCharacter1.SpecialMeter = 0f;
            playerCharacter1.UsableMoveSet.DisableMoveByType(Move.Type.Special);
        }

        if ((movePlayer2 != null) && (movePlayer2.MoveType == Move.Type.Special))
        {
            playerCharacter2.Effect.DoSpecial();
            playerCharacter2.SpecialMeter = 0f;
            playerCharacter2.UsableMoveSet.DisableMoveByType(Move.Type.Special);
        }

        // Preliminary damage and special calculations
        var damageToPlayer1 = 0f;
        var damageToPlayer2 = 0f;

        var special1 = _baseSpecialGain;
        special1 *= playerCharacter1.Effect.GetSpecialMeterGainModifier();
        special1 *= playerCharacter2.Effect.GetSpecialMeterGivenModifier();

        var special2 = _baseSpecialGain;
        special2 *= playerCharacter2.Effect.GetSpecialMeterGainModifier();
        special2 *= playerCharacter1.Effect.GetSpecialMeterGivenModifier();

        // Check who won and apply updates
        if (player1Wins && !player2Wins)
        {
            damageToPlayer2 = _baseDamage;
            damageToPlayer2 *= playerCharacter1.Effect.GetOutgoingDamageModifier();
            damageToPlayer2 *= playerCharacter2.Effect.GetIncomingDamageModifier();
            special2 *= _specialGainOnLossModifier;
            playerCharacter1.IncrementComboCount();
            playerCharacter2.ResetComboCount();
        }
        else if (!player1Wins && player2Wins)
        {
            damageToPlayer1 = _baseDamage;
            damageToPlayer1 *= playerCharacter2.Effect.GetOutgoingDamageModifier();
            damageToPlayer1 *= playerCharacter1.Effect.GetIncomingDamageModifier();
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

        // Check if specials should be enabled
        if (playerCharacter1.SpecialMeter >= 100f)
        {
            playerCharacter1.UsableMoveSet.EnableMoveByType(Move.Type.Special);
        }
        if (playerCharacter2.SpecialMeter >= 100f)
        {
            playerCharacter2.UsableMoveSet.EnableMoveByType(Move.Type.Special);
        }

        // Execute queued commands
        foreach (CombatCommandBase command in _combatCommands.Where(c => c.Round == _turnNumber))
        {
            command.Execute();
        }

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
