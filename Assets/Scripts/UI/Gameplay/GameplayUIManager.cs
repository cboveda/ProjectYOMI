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

    public ProgressBar roundTimerBar;
    public TMP_Text roundTimerText;

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
}

