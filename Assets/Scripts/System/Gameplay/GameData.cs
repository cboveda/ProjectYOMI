using Unity.Netcode;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking.Types;
using System;
using System.Linq;
using System.Collections.Generic;

public class GameData : NetworkBehaviour
{
    #region properties
    //Singleton
    private static GameData _instance;
    public static GameData Instance { get => _instance; }

    //Dependencies
    [SerializeField] private CharacterDatabase _characterDatabase;
    [SerializeField] private CharacterMoveDatabase _characterMoveDatabase;
    [SerializeField] private CharacterSpawner _characterSpawner;
    [SerializeField] private GameUIManager _gameUIManager;

    //Game Configuration
    [SerializeField] private float _baseDamage = 10f;
    [SerializeField] private float _baseSpecialGain = 25f;

    //Character/Player information
    private Character _characterPlayer1;
    private Character _characterPlayer2;
    private int _characterIdPlayer1;
    private int _characterIdPlayer2;
    private NetworkVariable<byte> _usableMoveListPlayer1 = new(0);
    private NetworkVariable<byte> _usableMoveListPlayer2 = new(0);
    private NetworkVariable<float> _healthPlayer1 = new(0);
    private NetworkVariable<float> _healthPlayer2 = new(0);
    private NetworkVariable<float> _specialMeterPlayer1 = new(0);
    private NetworkVariable<float> _specialMeterPlayer2 = new(0);
    private NetworkVariable<int> _actionPlayer1 = new(-1);
    private NetworkVariable<int> _actionPlayer2 = new(-1);
    private NetworkVariable<int> _comboCountPlayer1 = new(0);
    private NetworkVariable<int> _comboCountPlayer2 = new(0);
    private NetworkVariable<ulong> _clientIdPlayer1 = new(9999);
    private NetworkVariable<ulong> _clientIdPlayer2 = new(9999);
    
    //General data
    private RoundDataBuilder _roundDataBuilder;
    private List<CombatCommandBase> _combatCommands;
    private List<RoundData> _roundDataList;
    private NetworkVariable<int> _roundNumber = new(0);
    private bool _displayDebugMenu = false;

    //Getters and Setters
    public CharacterMoveDatabase CharacterMoveDatabase { get { return _characterMoveDatabase; } }
    public int CharacterIdPlayer1 { get; set; }
    public int CharacterIdPlayer2 { get; set; }
    public List<CombatCommandBase> CombatCommands { get { return _combatCommands; } }
    public List<RoundData> RoundDataList { get { return _roundDataList; } }
    public NetworkVariable<byte> UsableMoveListPlayer1 { get { return _usableMoveListPlayer1; } }
    public NetworkVariable<byte> UsableMoveListPlayer2 { get { return _usableMoveListPlayer2; } }
    public NetworkVariable<float> HealthPlayer1 { get { return _healthPlayer1; } }
    public NetworkVariable<float> HealthPlayer2 { get { return _healthPlayer2; } }
    public NetworkVariable<float> SpecialMeterPlayer1 { get { return _specialMeterPlayer1; } }
    public NetworkVariable<float> SpecialMeterPlayer2 { get { return _specialMeterPlayer2; } }
    public NetworkVariable<int> ActionPlayer1 { get { return _actionPlayer1; } }
    public NetworkVariable<int> ActionPlayer2 { get { return _actionPlayer2; } }
    public NetworkVariable<int> ComboCountPlayer1 { get { return _comboCountPlayer1; } }
    public NetworkVariable<int> ComboCountPlayer2 { get { return _comboCountPlayer2; } }
    public NetworkVariable<int> RoundNumber { get { return _roundNumber; } }
    public NetworkVariable<ulong> ClientIdPlayer1 { get { return _clientIdPlayer1; } set { _clientIdPlayer1 = value; } }
    public NetworkVariable<ulong> ClientIdPlayer2 { get { return _clientIdPlayer2; } set { _clientIdPlayer2 = value; } }
    #endregion

    #region lifecycle methods
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        _roundDataBuilder = new RoundDataBuilder();
        _roundDataList = new List<RoundData>();
        _combatCommands = new List<CombatCommandBase>();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
        {
            return;
        }

        _roundDataList.Clear();
    }
    #endregion

    #region player initialization and setters
    public void InitializePlayerCharacter(int playerNumber, ulong clientId, int characterId)
    {
        Debug.Log("Called " + playerNumber + " " + clientId);
        if (playerNumber == 1)
        {
            _clientIdPlayer1.Value = clientId;
            _characterIdPlayer1 = characterId;
            _characterPlayer1 = _characterDatabase.GetCharacterById(_characterIdPlayer1);
            if (_characterPlayer1 != null)
            {
                _healthPlayer1.Value = _characterPlayer1.MaximumHealth;
                _gameUIManager.SetPlayer1MaximumHealthClientRpc(_characterPlayer1.MaximumHealth);
                _gameUIManager.SetPlayer1NameClientRpc(_characterPlayer1.DisplayName);
                foreach (CharacterMove.Type type in Enum.GetValues(typeof(CharacterMove.Type)))
                {
                    byte isUsable = (byte)(_characterPlayer1.CharacterMoveSet.GetMoveByType(type).UsableByDefault ? 1 : 0);
                    _usableMoveListPlayer1.Value |= (byte)((byte)type * isUsable);
                }
            }
        }
        else if (playerNumber == 2)
        {
            _clientIdPlayer2.Value = clientId;
            _characterIdPlayer2 = characterId;
            _characterPlayer2 = _characterDatabase.GetCharacterById(_characterIdPlayer2);
            if (_characterPlayer2 != null)
            {
                _healthPlayer2.Value = _characterPlayer2.MaximumHealth;
                _gameUIManager.SetPlayer2MaximumHealthClientRpc(_characterPlayer2.MaximumHealth);
                _gameUIManager.SetPlayer2NameClientRpc(_characterPlayer2.DisplayName);
                foreach (CharacterMove.Type type in Enum.GetValues(typeof(CharacterMove.Type)))
                {
                    byte isUsable = (byte)(_characterPlayer2.CharacterMoveSet.GetMoveByType(type).UsableByDefault ? 1 : 0);
                    _usableMoveListPlayer2.Value |= (byte)((byte)type * isUsable);
                }
            }
        }
    }
    public void UpdateHealthPlayer1(int delta)
    {
        _healthPlayer1.Value -= delta;
    }

    public void UpdateHealthPlayer2(int delta)
    {
        _healthPlayer2.Value -= delta;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SubmitPlayerActionServerRpc(int moveId, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        if (clientId == _clientIdPlayer1.Value)
        {
            _actionPlayer1.Value = moveId;
        }
        else if (clientId == _clientIdPlayer2.Value)
        {
            _actionPlayer2.Value = moveId;
        }
    }
    #endregion

    #region combat evaluation
    // This is a mess, but it's just to test functionality
    // Major refactoring opportunity!
    public void EvaluateRound()
    {
        _roundNumber.Value += 1;
        _roundDataBuilder.StartNewRoundData();

        // Read Inputs
        var movePlayer1 = _characterMoveDatabase.GetMoveById(_actionPlayer1.Value);
        var movePlayer2 = _characterMoveDatabase.GetMoveById(_actionPlayer2.Value);

        // Log and check for specials
        if (movePlayer1)
        {
            _roundDataBuilder.SetMoveIdPlayer1(movePlayer1.Id);
            if (movePlayer1.MoveType == CharacterMove.Type.Special)
            {
                _characterPlayer1.Effect.DoSpecial(this, _clientIdPlayer1.Value);
                _specialMeterPlayer1.Value = 0f;
                unchecked
                {
                    _usableMoveListPlayer1.Value &= (byte)~(byte)CharacterMove.Type.Special;
                }
            }
        }
        if (movePlayer2)
        {
            _roundDataBuilder.SetMoveIdPlayer2(movePlayer2.Id);
            if (movePlayer2.MoveType == CharacterMove.Type.Special)
            {
                _characterPlayer2.Effect.DoSpecial(this, _clientIdPlayer2.Value);
                _specialMeterPlayer2.Value = 0f;
                unchecked
                {
                    _usableMoveListPlayer2.Value &= (byte)~(byte)CharacterMove.Type.Special;
                }
            }
        } 

        // If neither submitted
        if (!movePlayer1 && !movePlayer2)
        {
            _gameUIManager.DisplayRoundResultClientRpc("None selected!", "", "None selected!", "", "Draw!");
            return;
        }

        // If one player submitted and the other didnt
        else if (movePlayer1 && !movePlayer2)
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                movePlayer1.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer1.MoveType),
                "None selected!",
                "",
                "Player 1 wins round");

            var damage = _baseDamage;
            damage *= _characterPlayer1.Effect.GetOutgoingDamageModifier(this, _clientIdPlayer1.Value);
            damage *= _characterPlayer2.Effect.GetIncomingDamageModifier(this, _clientIdPlayer2.Value);
            _healthPlayer2.Value -= damage;

            var special1 = _baseSpecialGain;
            special1 *= _characterPlayer1.Effect.GetSpecialMeterGainModifier(this, _clientIdPlayer1.Value);
            special1 *= _characterPlayer2.Effect.GetSpecialMeterGivenModifier(this, _clientIdPlayer2.Value);
            _specialMeterPlayer1.Value += special1;

            var special2 = _baseSpecialGain;
            special2 *= _characterPlayer2.Effect.GetSpecialMeterGainModifier(this, _clientIdPlayer2.Value);
            special2 *= _characterPlayer1.Effect.GetSpecialMeterGivenModifier(this, _clientIdPlayer1.Value);
            _specialMeterPlayer2.Value += special2 * 0.5f;

            _comboCountPlayer1.Value++;
            _comboCountPlayer2.Value = 0;

            _roundDataBuilder.SetDamageToPlayer2(damage);
        }
        else if (!movePlayer1 && movePlayer2)
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                "None selected!",
                "",
                movePlayer2.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer2.MoveType),
                "Player 2 wins round");

            var damage = _baseDamage;
            damage *= _characterPlayer2.Effect.GetOutgoingDamageModifier(this, _clientIdPlayer2.Value);
            damage *= _characterPlayer1.Effect.GetIncomingDamageModifier(this, _clientIdPlayer1.Value);
            _healthPlayer1.Value -= damage;

            var special1 = _baseSpecialGain;
            special1 *= _characterPlayer1.Effect.GetSpecialMeterGainModifier(this, _clientIdPlayer1.Value);
            special1 *= _characterPlayer2.Effect.GetSpecialMeterGivenModifier(this, _clientIdPlayer2.Value);
            _specialMeterPlayer1.Value += special1 * 0.5f;

            var special2 = _baseSpecialGain;
            special2 *= _characterPlayer2.Effect.GetSpecialMeterGainModifier(this, _clientIdPlayer2.Value);
            special2 *= _characterPlayer1.Effect.GetSpecialMeterGivenModifier(this, _clientIdPlayer1.Value);
            _specialMeterPlayer2.Value += special2;

            _comboCountPlayer1.Value = 0;
            _comboCountPlayer2.Value++;

            _roundDataBuilder.SetDamageToPlayer1(damage);
        }

        // Check defeat types
        else if (movePlayer1.Defeats(movePlayer2.MoveType))
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                movePlayer1.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer1.MoveType),
                movePlayer2.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer2.MoveType),
                "Player 1 wins round");

            var damage = _baseDamage;
            damage *= _characterPlayer1.Effect.GetOutgoingDamageModifier(this, _clientIdPlayer1.Value);
            damage *= _characterPlayer2.Effect.GetIncomingDamageModifier(this, _clientIdPlayer2.Value);
            _healthPlayer2.Value -= damage;

            var special1 = _baseSpecialGain;
            special1 *= _characterPlayer1.Effect.GetSpecialMeterGainModifier(this, _clientIdPlayer1.Value);
            special1 *= _characterPlayer2.Effect.GetSpecialMeterGivenModifier(this, _clientIdPlayer2.Value);
            _specialMeterPlayer1.Value += special1;

            var special2 = _baseSpecialGain;
            special2 *= _characterPlayer2.Effect.GetSpecialMeterGainModifier(this, _clientIdPlayer2.Value);
            special2 *= _characterPlayer1.Effect.GetSpecialMeterGivenModifier(this, _clientIdPlayer1.Value);
            _specialMeterPlayer2.Value += special2 * 0.5f;

            _comboCountPlayer1.Value++;
            _comboCountPlayer2.Value = 0;

            _roundDataBuilder.SetDamageToPlayer2(damage);
        }
        else if (movePlayer2.Defeats(movePlayer1.MoveType))
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                movePlayer1.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer1.MoveType),
                movePlayer2.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer2.MoveType),
                "Player 2 wins round");

            var damage = _baseDamage;
            damage *= _characterPlayer2.Effect.GetOutgoingDamageModifier(this, _clientIdPlayer2.Value);
            damage *= _characterPlayer1.Effect.GetIncomingDamageModifier(this, _clientIdPlayer1.Value);
            _healthPlayer1.Value -= damage;

            var special1 = _baseSpecialGain;
            special1 *= _characterPlayer1.Effect.GetSpecialMeterGainModifier(this, _clientIdPlayer1.Value);
            special1 *= _characterPlayer2.Effect.GetSpecialMeterGivenModifier(this, _clientIdPlayer2.Value);
            _specialMeterPlayer1.Value += special1 * 0.5f;

            var special2 = _baseSpecialGain;
            special2 *= _characterPlayer2.Effect.GetSpecialMeterGainModifier(this, _clientIdPlayer2.Value);
            special2 *= _characterPlayer1.Effect.GetSpecialMeterGivenModifier(this, _clientIdPlayer1.Value);
            _specialMeterPlayer2.Value += special2;

            _comboCountPlayer1.Value = 0;
            _comboCountPlayer2.Value++;

            _roundDataBuilder.SetDamageToPlayer1(damage);
        }

        // If both players submitted the same move type or neither wins (special case?)
        else if (movePlayer1.MoveType == movePlayer2.MoveType)
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                movePlayer1.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer1.MoveType),
                movePlayer2.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer2.MoveType),
                "Draw!");

            var special1 = _baseSpecialGain;
            special1 *= _characterPlayer1.Effect.GetSpecialMeterGainModifier(this, _clientIdPlayer1.Value);
            special1 *= _characterPlayer2.Effect.GetSpecialMeterGivenModifier(this, _clientIdPlayer2.Value);
            _specialMeterPlayer1.Value += special1 * 0.5f;

            var special2 = _baseSpecialGain;
            special2 *= _characterPlayer2.Effect.GetSpecialMeterGainModifier(this, _clientIdPlayer2.Value);
            special2 *= _characterPlayer1.Effect.GetSpecialMeterGivenModifier(this, _clientIdPlayer1.Value);
            _specialMeterPlayer2.Value += special2 * 0.5f;

            var damage = _baseDamage * 0.5f;
            _healthPlayer1.Value -= damage;
            _healthPlayer2.Value -= damage;
            _roundDataBuilder.SetDamageToPlayer1(damage);
            _roundDataBuilder.SetDamageToPlayer2(damage);

        }

        // Check if specials should be enabled
        if (_specialMeterPlayer1.Value >= 100f)
        {
            _usableMoveListPlayer1.Value |= (byte)CharacterMove.Type.Special;
        }
        if (_specialMeterPlayer2.Value >= 100f)
        {
            _usableMoveListPlayer2.Value |= (byte)CharacterMove.Type.Special;
        }

        // Execute queued commands
        foreach (CombatCommandBase command in _combatCommands.Where(c => c.Round == _roundNumber.Value))
        {
            command.Execute(this);
        }

        // Reset moves
        _actionPlayer1.Value = -1;
        _actionPlayer2.Value = -1;

        // Log combat
        _roundDataList.Add(_roundDataBuilder.GetRoundData());
    }
    #endregion

    #region debug UI

    private void OnGUI()
    {
        if (!IsServer) return;

        if (_displayDebugMenu)
        {
            GUILayout.BeginArea(new Rect(10, 10, 200, 400), GUI.skin.box);
            if (GUILayout.Button("Hide Debug")) _displayDebugMenu = false;
            if (GUILayout.Button("End Game")) _healthPlayer2.Value = 0f;
            GUILayout.Label($"Round Number: {RoundNumber.Value}");
            foreach (RoundData round in _roundDataList)
            {
                GUILayout.Label(
                    "P1: " +
                    Enum.GetName(typeof(CharacterMove.Type), _characterMoveDatabase.GetMoveById(round.MoveIdPlayer1)?.MoveType) +
                    ", P2: " +
                    Enum.GetName(typeof(CharacterMove.Type), _characterMoveDatabase.GetMoveById(round.MoveIdPlayer2)?.MoveType));
            }

            GUILayout.EndArea();
        }
        else
        {
            if (GUILayout.Button("Display Debug")) _displayDebugMenu = true;
        }
    }
    #endregion
}
