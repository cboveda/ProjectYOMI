using Unity.Netcode;

public interface IPlayerCharacter
{
    int Action { get; set; }
    Character Character { get; }
    ulong ClientId { get; set; }
    int ComboCount { get; set; }
    CharacterBaseEffect Effect { get; }
    IGameUIManager GameUIManager { set; }
    float Health { get; set; }
    PlayerData PlayerData { get; set; }
    PlayerMovementController PlayerMovementController { get; }
    int PlayerNumber { get; set; }
    int Position { get; set; }
    float SpecialMeter { get; set; }
    IUsableMoveSet UsableMoveSet { get; }

    void DecreaseHealth(float value);
    void IncreaseSpecialMeter(float value);
    void IncrementComboCount();
    void ResetAction();
    void ResetComboCount();
    void SubmitPlayerActionServerRpc(int moveId, ServerRpcParams serverRpcParams = default);
}