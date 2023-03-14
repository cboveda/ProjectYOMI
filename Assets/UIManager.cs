using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine;
using System;

public class UIManager : NetworkBehaviour
{
    public Slider player1HealthSlider;
    public Slider player2HealthSlider;

    public void SetPlayer1HealthSliderValue(int oldValue, int newValue)
    {
        player1HealthSlider.value = newValue;
        Debug.Log("Set player 1's health bar slider value.");
    }

    public void SetPlayer2HealthSliderValue(int oldValue, int newValue)
    {
        player1HealthSlider.value = newValue;
        Debug.Log("Set player 2's health bar slider value.");
    }

}
