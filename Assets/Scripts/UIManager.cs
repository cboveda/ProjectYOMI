using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class UIManager : NetworkBehaviour
{
    public static UIManager Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (GameManager.Instance.RoundTimerEnabled)
        {
            RoundTimerSlider.value = GameManager.Instance.RoundTimer;
        }

        ReadyText.text = $"Players Ready: {GameStateManager.Instance.PlayersReady}/2";
    }
    public void Start()
    {
        ReadyButton.onClick.AddListener(() =>
        {
            GameStateManager.Instance.PlayerReady();
            ReadyButton.interactable = false;
        });
    }


    #region Healthbars
    public Slider player1HealthSlider;
    public Slider player2HealthSlider;

    public void UpdateHealth()
    {
        if (GameManager.Instance.Players.Count > 0) player1HealthSlider.value = GameManager.Instance.Players.Values.ElementAt(0).Health;
        if (GameManager.Instance.Players.Count == 2) player2HealthSlider.value = GameManager.Instance.Players.Values.ElementAt(1).Health;
    }
    #endregion


    public Slider RoundTimerSlider;
    public TMP_Text RoundTimerText;

    [SerializeField]
    private float roundLength;

    public void SetRoundTimer(float newValue)
    {
        RoundTimerSlider.value = newValue;
        RoundTimerText.text = newValue.ToString("0.0");
    }

    public Button ReadyButton;
    public TMP_Text ReadyText;
    public Animator Countdown;
    public bool ReadyInterfaceActive = false;

    public void HideReadyInterface()
    {
        ReadyText.gameObject.SetActive(false);
        ReadyButton.gameObject.SetActive(false);
        ReadyInterfaceActive = false;
    }

    public void ShowReadyInterface()
    {
        ReadyText.gameObject.SetActive(true);
        ReadyButton.gameObject.SetActive(true);
        ReadyInterfaceActive = true;
    }

    public void StartCountdownAnimation()
    {
        Countdown.gameObject.SetActive(true);
        Countdown.SetTrigger("PlayCountdown");
    }
}
