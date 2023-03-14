using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Collections.Generic;

public class UIManager : NetworkBehaviour
{
    public Slider player1HealthSlider;
    public Slider player2HealthSlider;

    public List<ulong> Players;

    public static UIManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

    public static void AddPlayer(ulong uid)
    {
        Debug.Log("Adding player: " + uid);
        UIManager.instance.Players.Add(uid);
    }

    public void UpdateHealth(ulong uid, int newValue)
    {
        if (uid == Players[0])
        {
            player1HealthSlider.value = newValue;
        }
        else if (uid == Players[1])
        {
            player2HealthSlider.value = newValue;
        }
    }

}
