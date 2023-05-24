using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TurnHistoryContent : MonoBehaviour
{
    [SerializeField] private GameObject _turnHistoryRowPrefab;
    private List<GameObject> _turnHistoryRows = new();
    private IDatabase _database;

    [Inject]
    public void Construct(IDatabase database)
    {
        _database = database;
    }

    public void AddTurnHistoryRow(TurnResult turnResult)
    {
        var newRow = Instantiate(_turnHistoryRowPrefab, transform);
        newRow.transform.SetSiblingIndex(1);
        _turnHistoryRows.Add(newRow);
        var newRowComponent = newRow.GetComponent<TurnHistoryRow>();

        var player1Move = _database.Moves.GetMoveById(turnResult.PlayerData1.Action);
        var player1MoveName = (player1Move == null) ? "" : player1Move.MoveName;
        newRowComponent.SetPlayer1MoveName(player1MoveName);

        var player2Move = _database.Moves.GetMoveById(turnResult.PlayerData2.Action);
        var player2MoveName = (player2Move == null) ? "" : player2Move.MoveName;
        newRowComponent.SetPlayer2MoveName(player2MoveName);

        if (turnResult.DamageToPlayer1 ==  turnResult.DamageToPlayer2)
        {
            newRowComponent.SetWinnerDisplayState(TurnHistoryRow.WinnerDisplayStates.turnWasDraw);
        }
        else if (turnResult.DamageToPlayer2 > turnResult.DamageToPlayer1)
        {
            newRowComponent.SetWinnerDisplayState(TurnHistoryRow.WinnerDisplayStates.player1Wins);
        }
        else if (turnResult.DamageToPlayer1 > turnResult.DamageToPlayer2)
        {
            newRowComponent.SetWinnerDisplayState(TurnHistoryRow.WinnerDisplayStates.player2Wins);
        }
    }
}
