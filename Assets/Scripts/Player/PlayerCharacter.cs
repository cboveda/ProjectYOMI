using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField] private Character _character;
    private PlayerData _playerData;
    private CharacterBaseEffect _characterBaseEffect;
    private ulong _clientId;
    private int _playerNumber;
    private UsableMoveSet _usableMoveSet;

    public Character Character { get => _character; }
    public ulong ClientId { get => _clientId; set => _clientId = value; }
    public PlayerData PlayerData { get => _playerData; }
    public UsableMoveSet UsableMoveSet { get => _usableMoveSet; }
    public CharacterBaseEffect Effect { get => _characterBaseEffect; }
    public int PlayerNumber { get => _playerNumber; set => _playerNumber = value; }

    void Awake()
    {
        _playerData = ScriptableObject.CreateInstance<PlayerData>();
        _playerData.Health = _character.MaximumHealth;
        _playerData.SpecialMeter = 0;
        _playerData.Action = CharacterMove.NO_MOVE;
        _playerData.ComboCount = 0;

        _usableMoveSet = ScriptableObject.CreateInstance<UsableMoveSet>();
        _usableMoveSet.Initialize(_character.CharacterMoveSet);

        _characterBaseEffect = GetComponent<CharacterBaseEffect>();
    }

    public void ResetAction()
    {
        _playerData.Action = CharacterMove.NO_MOVE;
    }
}
