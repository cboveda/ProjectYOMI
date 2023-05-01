using Unity.Netcode;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
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

    public float Health
    {
        get
        {
            return _playerData.Health;
        }
        set
        {
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
            GameUIManager.Instance.UpdateActiveSelectionButtonClientRpc(previous, value, clientRpcParams);
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
            _playerData = new PlayerData
            {
                Health = _playerData.Health,
                SpecialMeter = _playerData.SpecialMeter,
                Action = _playerData.Action,
                ComboCount = value
            };
        }
    }

    void Awake()
    {
        _playerData = new PlayerData(health: _character.MaximumHealth);
        _usableMoveSet = GetComponent<UsableMoveSet>();
        _characterBaseEffect = GetComponent<CharacterBaseEffect>();
    }

    public void ResetAction()
    {
        Action = CharacterMove.NO_MOVE;
    }
}