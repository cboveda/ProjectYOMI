using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    #region Actions
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
    #endregion

    #region Health
    public NetworkVariable<int> Health = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone);

    [ServerRpc]
    public void ChangeHealthServerRpc(int value)
    {
        Health.Value -= value;
    }

    void OnHealthChange(int oldValue, int newValue)
    {
        Debug.LogFormat($"Client {OwnerClientId} health changed from {oldValue} to {newValue}");
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Health.Value = 100;
        }
        if (!IsLocalPlayer)
        {
            GetComponentInChildren<Canvas>().enabled = false;
        }

        Health.OnValueChanged += OnHealthChange;
        Health.OnValueChanged += SetPlayer1HealthSliderValue;
    }

    public override void OnNetworkDespawn()
    {
        Health.OnValueChanged -= OnHealthChange;
        Health.OnValueChanged -= SetPlayer1HealthSliderValue;
    }

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

    #endregion
}
