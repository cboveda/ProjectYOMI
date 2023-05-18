using Unity.Netcode;

public interface IPlayerCharacter
{
    bool ComboIsFresh { get; set; }
    Character Character { get; }
    CharacterBaseEffect Effect { get; }
    float Health { get; set; }
    float SpecialMeter { get; set; }
    IGameUIManager GameUIManager { set; }
    int Action { get; set; }
    int ComboCount { get; set; }
    int PlayerNumber { get; set; }
    int Position { get; set; }
    IUsableMoveSet UsableMoveSet { get; }
    PlayerData PlayerData { get; set; }
    PlayerMovementController PlayerMovementController { get; }
    ulong ClientId { get; set; }

    void DecreaseHealth(float value);
    void IncreaseSpecialMeter(float value);
    void IncrementComboCount();
    void ResetAction();
    void ResetComboCount();
    void SubmitPlayerActionServerRpc(int moveId, ServerRpcParams serverRpcParams = default);
}