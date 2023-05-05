using Unity.Netcode;

public interface IGameUIManager
{ 
    void DisplayGameResultClientRpc(string result);
    void DisplayRoundResult(TurnData turnData);
    void HideRoundResultClientRpc();
    void OnNetworkDespawn();
    void OnNetworkSpawn();
    void RegisterPlayerCharacter(int playerNumber, ulong clientId);
    void SetUpLocalActionButtonsClientRpc(ClientRpcParams clientRpcParams = default);
    void StartRoundTimerClientRpc(float duration);
    void SubmitPlayerAction(int id);
    void UpdateActiveSelectionButtonClientRpc(int previousValue, int newValue, ClientRpcParams clientRpcParams = default);
    void UpdatePlayer1Combo(int newValue);
    void UpdatePlayer1Health(float newValue);
    void UpdatePlayer1Special(float newValue);
    void UpdatePlayer2Combo(int newValue);
    void UpdatePlayer2Health(float newValue);
    void UpdatePlayer2Special(float newValue);
}