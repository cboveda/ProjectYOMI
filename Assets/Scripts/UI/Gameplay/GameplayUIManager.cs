using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameplayUIManager : MonoBehaviour
{
    public static GameplayUIManager Instance;

    public TMP_Text player1Name;
    public TMP_Text player2Name;

    public ProgressBar player1Health;
    public ProgressBar player2Health;

    public RoundTimer roundTimer;

    public PlayerControls playerControls;

    private void Awake()
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

    public void StartRoundTimer(float duration)
    {
        roundTimer.StartTimer(duration);
    }
}

