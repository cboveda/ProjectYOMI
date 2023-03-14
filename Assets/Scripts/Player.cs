using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
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
    public NetworkVariable<ulong> playerId = new NetworkVariable<ulong>();

    public ulong PlayerId {  get { return playerId.Value; } set { playerId.Value = value; } }

    [ServerRpc]
    public void ChangeHealthServerRpc(int value)
    {
        Health.Value -= value;
    }

    void OnHealthChange(int oldValue, int newValue)
    {
        UIManager.instance.UpdateHealth(PlayerId, newValue);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer) PlayerId = OwnerClientId;

        Debug.Log("Spawned player: " + PlayerId);

        UIManager.AddPlayer(PlayerId);
        Health.OnValueChanged += OnHealthChange;
    }

    public override void OnNetworkDespawn()
    {
        Health.OnValueChanged -= OnHealthChange;
    }

    #endregion
}
