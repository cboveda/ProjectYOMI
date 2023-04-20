using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameplayUIManager : NetworkBehaviour
{
    public static GameplayUIManager Instance;

    [SerializeField] private TMP_Text _player1Name;
    [SerializeField] private TMP_Text _player2Name;

    [SerializeField] private ProgressBar _player1Health;
    [SerializeField] private ProgressBar _player2Health;

    [SerializeField] private RoundTimer _roundTimer;

    [SerializeField] private PlayerControls _playerControls;

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

    [ClientRpc]
    public void StartRoundTimerClientRpc(float duration)
    {
        _roundTimer.StartTimer(duration);
    }
}

