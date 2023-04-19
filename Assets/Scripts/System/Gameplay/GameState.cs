using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class GameState : NetworkBehaviour
{
    public static GameState Instance { get; private set; }

    readonly NetworkVariable<byte> state = new NetworkVariable<byte>();
    public byte State { get { return state.Value; } set { state.Value = value; } }

    public enum States : byte
    {
        Init,
        NotReady,
        StartGame,
        RoundActive,
        RoundResolve,
        EndGame        
    }

    public event Action OnStateChangedNotReady;
    public event Action OnStateChangedStartGame;
    public event Action OnStateChangedRoundActive;
    public event Action OnStateChangedRoundResolve;
    public event Action OnStateChangedEndGame;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        GUILayout.Label($"State: {Enum.GetName(typeof(States), State)}");
        GUILayout.EndArea();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        state.OnValueChanged += OnStateChanged;
        State = (byte)States.Init;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        state.OnValueChanged -= OnStateChanged;
    }

    public void OnStateChanged(byte oldState, byte newState)
    {
        switch (newState)
        {
            case (byte)States.NotReady:
                OnStateChangedNotReady.Invoke();
                break;
            case (byte)States.StartGame:
                OnStateChangedStartGame.Invoke();
                break;
            case (byte)States.RoundActive:
                OnStateChangedRoundActive.Invoke();
                break;
            case (byte)States.RoundResolve:
                OnStateChangedRoundResolve.Invoke();
                break;
            case (byte)States.EndGame:
                OnStateChangedEndGame.Invoke();
                break;
            default: break;
        }
    }

    public void AdvanceState(bool endGame = false)
    {
        switch (State)
        {
            case (byte)States.Init:
                State = (byte)States.NotReady;
                break;
            case (byte)States.NotReady:
                State = (byte)States.StartGame;
                break;
            case (byte)States.StartGame:
                State = (byte)States.RoundActive;
                break;
            case (byte)States.RoundActive:
                State = (byte)States.RoundResolve;
                break;
            case (byte)States.RoundResolve:
                if (endGame)
                {
                    State = (byte)States.EndGame;
                }
                else
                {
                    State = (byte)States.RoundActive;
                }
                break;
            case (byte)States.EndGame:
                Debug.Log("Game Over");
                break;
            default: break;
        }
    }
}
