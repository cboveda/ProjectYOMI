using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameStateManager : NetworkBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [SerializeField] private float startGameStateDuration;
    [SerializeField] private float roundActiveStateDuration;
    [SerializeField] private float resolveRoundStateDuration;
    [SerializeField] private float endGameStateDuration;
    [SerializeField] private float timer = 0;
    [SerializeField] private bool timerActive = false;

    NetworkVariable<byte> state = new NetworkVariable<byte>();
    public byte State { get { return state.Value; } set { state.Value = value; } }

    NetworkVariable<int> playersReady = new NetworkVariable<int>(0);
    public int PlayersReady { get { return playersReady.Value; } set { playersReady.Value = value; } }

    public enum States : byte
    {
        NotReady = 0,
        StartGame = 1,
        RoundActive = 2,
        ResolveRound = 3,
        EndGame = 4
    }

    public override void OnNetworkSpawn()
    {
        State = (byte)States.NotReady;

        state.OnValueChanged += OnStateChange;
    }

    public override void OnNetworkDespawn()
    {
        state.OnValueChanged -= OnStateChange;
    }

    public void OnStateChange(byte oldState, byte newState)
    {
        switch(State)
        {
            case (byte)States.NotReady:
                // Display ready UI
                // Display game UI
                // Set player healths
                if (IsServer)
                {
                    timerActive = false;
                }
                break;
            case (byte)States.StartGame:
                // Hide ready UI
                // Display 3,2,1... Countdown

                // (server) start timer
                if (IsServer)
                {
                    timerActive = true;
                }
                break;
            case (byte)States.RoundActive:
                // Start round timer
                // Slowdown time

                // (server) start timer
                if (IsServer)
                {
                    timerActive = true;
                }
                break;
            case (byte)States.ResolveRound:
                // Read user inputs
                // Determine combat outcome
                // Interrupt player animations
                // Start resolve timer

                // (server) start timer
                if (IsServer)
                {
                    timerActive = true;
                }
                break;
            case (byte)States.EndGame:
                // Start end game timer
                // Start player victory/lose animations
                // Post game results

                // (server) start timer
                if (IsServer)
                {
                    timerActive = true;
                }
                break;
            default: break;
        }

    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (!IsServer) return;

        if (timerActive) timer += Time.unscaledDeltaTime;

        switch (State)
        {
            case (byte)States.NotReady:
                // If everyone is ready, move to StartGame
                if (PlayersReady == 2)
                {
                    State = (byte)States.StartGame;
                }
                break;
            case (byte)States.StartGame:
                // If countdown done, unlock player input and move to RoundActive
                if (timer >= startGameStateDuration)
                {
                    timer = 0;
                    State = (byte)States.RoundActive;
                }
                break;
            case (byte)States.RoundActive:
                // If round timer done, resume normal time and start player animations
                if (timer >= roundActiveStateDuration)
                {
                    timer = 0;
                    State = (byte)States.ResolveRound;
                }
                break;
            case (byte)States.ResolveRound:
                // If resolve timer done
                if (timer >= roundActiveStateDuration)
                {
                    timer = 0;

                    // If a player health < 0, move to EndGame
                    if (GameManager.Instance.CheckPlayerHealths())
                    {
                        State = (byte)States.EndGame;
                    }
                    // else, move to RoundActive
                    else
                    {
                        State = (byte)States.RoundActive;
                    }
                }
                break;
            case (byte)States.EndGame:
                // If end game timer done, move to NotReady
                if (timer >= endGameStateDuration)
                {
                    timer = 0;
                    State = (byte)States.ResolveRound;
                }
                break;
            default: break;
        }
    }
}
