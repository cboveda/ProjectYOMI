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

    [SerializeField]
    private Player player;

    public Button LightAttackButton;
    public Button HeavyAttackButton;
    public Button ParryButton;
    public Button GrabButton;

    public Slider player1HealthSlider;
    public Slider player2HealthSlider;

    public Slider RoundTimerSlider;
    public TMP_Text RoundTimerText;

    public Button ReadyButton;
    public TMP_Text ReadyText;
    public Animator Countdown;

    [SerializeField]
    private float roundLength;

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

    public void RegisterLocalPlayer(Player player)
    {
        this.player = player;
        LinkPlayerButtons();
    }

    private void LinkPlayerButtons()
    {
        LightAttackButton.onClick.AddListener(() =>
        {
            Debug.Log("Light attack"); 
            player.LightAttack();
        });
        HeavyAttackButton.onClick.AddListener(() =>
        {
            Debug.Log("Heavy attack"); 
            player.HeavyAttack();
        });
        ParryButton.onClick.AddListener(() =>
        {
            Debug.Log("Parry"); 
            player.Parry();
        });
        GrabButton.onClick.AddListener(() =>
        {
            Debug.Log("Grab"); 
            player.Grab();
        });
    }

    public void UpdateHealth()
    {
        if (GameManager.Instance.Players.Count > 0) player1HealthSlider.value = GameManager.Instance.Players.Values.ElementAt(0).Health;
        if (GameManager.Instance.Players.Count == 2) player2HealthSlider.value = GameManager.Instance.Players.Values.ElementAt(1).Health;
    }

    public void SetRoundTimer(float newValue)
    {
        RoundTimerSlider.value = newValue;
        RoundTimerText.text = newValue.ToString("0.0");
    }

    public void HideReadyInterface()
    {
        ReadyText.enabled = false;
        ReadyButton.gameObject.SetActive(false);
    }

    public void ShowReadyInterface()
    {
        ReadyText.enabled = true;
        ReadyButton.gameObject.SetActive(true);
    }

    public void StartCountdownAnimation()
    {
        Countdown.gameObject.SetActive(true);
        Countdown.SetTrigger("PlayCountdown");
    }

}
