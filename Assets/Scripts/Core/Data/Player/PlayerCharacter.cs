using Unity.Netcode;
using UnityEngine;
using Zenject;

public class PlayerCharacter : NetworkBehaviour, IPlayerCharacter
{
    [SerializeField] private Character _character;
    private IGameUIManager _gameUIManager;
    private int _playerNumber;
    private PlayerData _playerData;
    private PlayerMovementController _playerMovementController;
    private ulong _clientId;
    private IUsableMoveSet _usableMoveSet;
    public Character Character { get => _character; }
    public IGameUIManager GameUIManager { set => _gameUIManager = value; }
    public PlayerData PlayerData { get => _playerData; set => _playerData = value; }
    public PlayerMovementController PlayerMovementController { get => _playerMovementController; }
    public ulong ClientId { get => _clientId; set => _clientId = value; }
    public IUsableMoveSet UsableMoveSet { get => _usableMoveSet; }
    public int PlayerNumber
    {
        get => _playerNumber; set
        {
            _playerNumber = value;
            _playerMovementController.Direction = (value == 1) ?
                PlayerMovementController.KnockbackDirection.Left :
                PlayerMovementController.KnockbackDirection.Right;
        }
    }

    void Awake()
    {
        _playerData = new PlayerData(health: _character.MaximumHealth);
        _usableMoveSet = GetComponent<UsableMoveSet>();
        _playerMovementController = GetComponent<PlayerMovementController>();
    }

    public float Health
    {
        get
        {
            return _playerData.Health;
        }
        set
        {
            value = Mathf.Clamp(value, 0, _character.MaximumHealth);
            _playerData = new PlayerData
            {
                Health = value,
                SpecialMeter = _playerData.SpecialMeter,
                Action = _playerData.Action,
                ComboCount = _playerData.ComboCount,
                Position = _playerData.Position
            };
        }
    }

    public float SpecialMeter
    {
        get
        {
            return _playerData.SpecialMeter;
        }
        set
        {
            value = Mathf.Clamp(value, 0, 100);
            _playerData = new PlayerData
            {
                Health = _playerData.Health,
                SpecialMeter = value,
                Action = _playerData.Action,
                ComboCount = _playerData.ComboCount,
                Position = _playerData.Position
            };
        }
    }

    public int Action
    {
        get
        {
            return _playerData.Action;
        }
        set
        {
            var previous = _playerData.Action;
            _playerData = new PlayerData
            {
                Health = _playerData.Health,
                SpecialMeter = _playerData.SpecialMeter,
                Action = value,
                ComboCount = _playerData.ComboCount,
                Position = _playerData.Position
            };
            var clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { _clientId },
                }
            };
            _gameUIManager.UpdateActiveSelectionButtonClientRpc(previous, value, clientRpcParams);
        }
    }

    public int ComboCount
    {
        get
        {
            return _playerData.ComboCount;
        }
        set
        {
            value = Mathf.Clamp(value, 0, int.MaxValue);
            _playerData = new PlayerData
            {
                Health = _playerData.Health,
                SpecialMeter = _playerData.SpecialMeter,
                Action = _playerData.Action,
                ComboCount = value,
                Position = _playerData.Position
            };
        }
    }

    public int Position
    {
        get
        {
            return _playerData.Position;
        }
        set
        {
            _playerData = new PlayerData
            {
                Health = _playerData.Health,
                SpecialMeter = _playerData.SpecialMeter,
                Action = _playerData.Action,
                ComboCount = _playerData.ComboCount,
                Position = value
            };
        }
    }

    public void ResetAction()
    {
        Action = Move.NO_MOVE;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SubmitPlayerActionServerRpc(int moveId, ServerRpcParams serverRpcParams = default)
    {
        Action = moveId;
    }

    public void DecreaseHealth(float value)
    {
        Health -= value;
    }

    public void IncreaseSpecialMeter(float value)
    {
        SpecialMeter += value;
    }

    public void IncreasePosition(int value)
    {
        Position += value;
    }

    public void IncrementComboCount()
    {
        ComboCount++;
    }

    public void ResetComboCount()
    {
        ComboCount = 0;
    }
}
