using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public NetworkVariable<ulong> playerId = new NetworkVariable<ulong>();
    public ulong PlayerId { get { return playerId.Value; } set { playerId.Value = value; } }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            PlayerId = OwnerClientId;
        }

        if (IsOwner)
        {
            UIManager.Instance.RegisterLocalPlayer(this);
        }

        GameManager.AddPlayer(PlayerId, this);
        Debug.Log("Spawned player: " + PlayerId);

        if (GameManager.Instance.Players.Keys.ElementAt(0) == PlayerId)
        {
            transform.position = new Vector3(-2f, -1f, 0);
        }
        else
        {
            transform.position = new Vector3(2f, -1f, 0);
            GetComponent<SpriteRenderer>().flipX = true;
        }

        health.OnValueChanged += OnHealthChange;
    }

    public override void OnNetworkDespawn()
    {
        GameManager.RemovePlayer(PlayerId);
        health.OnValueChanged -= OnHealthChange;
    }

    #region Actions
    public NetworkVariable<byte> PlayerAction = new(0);

    public enum PlayerActions : byte
    {
        None = 0,
        LightAttack = 1,
        HeavyAttack = 2,
        Parry = 3,
        Grab = 4
    }

    public void LightAttack()
    {
        SubmitPlayerActionServerRpc(PlayerActions.LightAttack);
    }

    public void HeavyAttack()
    {
        SubmitPlayerActionServerRpc(PlayerActions.HeavyAttack);
    }

    public void Parry()
    {
        SubmitPlayerActionServerRpc(PlayerActions.Parry);
    }

    public void Grab()
    {
        SubmitPlayerActionServerRpc(PlayerActions.Grab);
    }

    [ServerRpc]
    public void SubmitPlayerActionServerRpc(PlayerActions action)
    {
        PlayerAction.Value = (byte)action;
    }
    #endregion

    #region Health
    public NetworkVariable<int> health = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone);
    public int Health { get { return health.Value; } set { health.Value = value; } }

    public void ChangeHealth(int value)
    {
        ChangeHealthServerRpc(10);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeHealthServerRpc(int value)
    {
        Health -= value;
    }

    void OnHealthChange(int oldValue, int newValue)
    {
        UIManager.Instance.UpdateHealth();
    }
    #endregion
}
