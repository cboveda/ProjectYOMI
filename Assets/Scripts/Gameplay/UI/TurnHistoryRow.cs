using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnHistoryRow : MonoBehaviour
{
    [SerializeField] private TMP_Text _player1MoveName;
    [SerializeField] private TMP_Text _player2MoveName;
    [SerializeField] private GameObject _player1WinsDisplay;
    [SerializeField] private GameObject _player2WinsDisplay;
    [SerializeField] private GameObject _turnWasDrawDisplay;

    public enum WinnerDisplayStates
    {
        player1Wins, 
        player2Wins, 
        turnWasDraw
    }

    public void SetPlayer1MoveName(string name)
    {
        _player1MoveName.text = name;
    }

    public void SetPlayer2MoveName(string name)
    {
        _player2MoveName.text = name;
    }

    public void SetWinnerDisplayState(WinnerDisplayStates state)
    {
        if (state == WinnerDisplayStates.player1Wins)
        {
            _player1WinsDisplay.SetActive(true);
            _player2WinsDisplay.SetActive(false);
            _turnWasDrawDisplay.SetActive(false);
            return;
        }

        if (state == WinnerDisplayStates.player2Wins)
        {
            _player1WinsDisplay.SetActive(false);
            _player2WinsDisplay.SetActive(true);
            _turnWasDrawDisplay.SetActive(false);
        }

        if (state == WinnerDisplayStates.turnWasDraw)
        {
            _player1WinsDisplay.SetActive(false);
            _player2WinsDisplay.SetActive(false);
            _turnWasDrawDisplay.SetActive(true);
        }
    }


}
