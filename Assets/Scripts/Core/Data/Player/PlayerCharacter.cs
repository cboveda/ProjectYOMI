using Unity.Netcode;
using UnityEngine;
using Zenject;

public class PlayerCharacter : NetworkBehaviour
{
    private IGameUIManager _gameUIManager;
    [SerializeField] private Character _character;
    private CharacterBaseEffect _characterBaseEffect;
    private int _playerNumber;
    private PlayerData _playerData;
    private ulong _clientId;
    private UsableMoveSet _usableMoveSet;

    public Character Character { get => _character; }
    public CharacterBaseEffect Effect { get => _characterBaseEffect; }
    public int PlayerNumber { get => _playerNumber; set => _playerNumber = value; }
    public PlayerData PlayerData { get => _playerData; set => _playerData = value; }
    public ulong ClientId { get => _clientId; set => _clientId = value; }
    public UsableMoveSet UsableMoveSet { get => _usableMoveSet; }
    public IGameUIManager GameUIManager { set => _gameUIManager = value; }

    void Awake()
    {
        _playerData = new PlayerData(health: _character.MaximumHealth);
        _usableMoveSet = GetComponent<UsableMoveSet>();
        _characterBaseEffect = GetComponent<CharacterBaseEffect>();
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
                ComboCount = _playerData.ComboCount
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
                ComboCount = _playerData.ComboCount
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
                ComboCount = _playerData.ComboCount
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
                ComboCount = value
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
        SpecialMeter -= value;
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
