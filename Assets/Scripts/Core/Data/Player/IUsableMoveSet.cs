using Unity.Netcode;

public interface IUsableMoveSet
{
    NetworkVariable<byte> Moves { get; }

    bool CheckEnabledByType(Move.Type type);
    void DisableMoveByType(Move.Type type);
    void EnableMoveByType(Move.Type type);
    void InitializeMoveSet(CharacterMoveSet moveSet);
    void OnNetworkSpawn();
}