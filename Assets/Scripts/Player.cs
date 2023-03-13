using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Header("UI Settings")]
    public TMP_Text p1HealthText;
    public TMP_Text p2HealthText;

    public NetworkVariable<int> Player1Health = new(0);
    public NetworkVariable<int> Player2Health = new(0);
    public NetworkVariable<byte> PlayerAction = new(0);


    public enum PlayerActions
    {
        LightAttack = 0,
        HeavyAttack = 1,
        Parry = 2,
        Grab = 3
    }

    public void LightAttack() 
    {
        Debug.Log("Light Attack");
        SubmitPlayerActionServerRpc(PlayerActions.LightAttack);
    }
    
    public void HeavyAttack() 
    {
        Debug.Log("Heavy Attack");
        SubmitPlayerActionServerRpc(PlayerActions.HeavyAttack);
    }

    public void Parry()
    {
        Debug.Log("Parry");
        SubmitPlayerActionServerRpc(PlayerActions.Parry);
    }

    public void Grab()
    {
        Debug.Log("Grab");
        SubmitPlayerActionServerRpc(PlayerActions.Grab);
    }

    [ServerRpc]
    public void SubmitPlayerActionServerRpc(PlayerActions action)
    {
        PlayerAction.Value = (byte)action;
    }


    [ServerRpc]
    public void SetPlayerHealthServerRpc(int player, int value)
    {
        switch (player)
        {
            case 1: Player1Health.Value = value; break;
            case 2: Player2Health.Value = value; break;
            default: break;
        }
    }

    private void OnEnable()
    {
        Player1Health.OnValueChanged += OnHealthChanged;
        Player2Health.OnValueChanged += OnHealthChanged;
    }

    private void OnDisable()
    {
        Player1Health.OnValueChanged -= OnHealthChanged;
        Player2Health.OnValueChanged -= OnHealthChanged;
    }
    private void OnHealthChanged(int oldValue, int newValue)
    {
        if (!IsClient) return;
        Debug.Log("Health changed!");
        p1HealthText.SetText("{0}", Player1Health.Value);
        p2HealthText.SetText("{0}", Player2Health.Value);
    }


}
