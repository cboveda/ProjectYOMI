using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterSpawner : NetworkBehaviour
{
    [SerializeField] private CharacterDatabase _characterDatabase;
    [SerializeField] private GameObject _player1SpawnLocation;
    [SerializeField] private GameObject _player2SpawnLocation;
    [SerializeField] private GameData _gameData;

    private bool _player1Spawned = false;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        foreach (var client in ServerManager.Instance.ClientData)
        {
            var character = _characterDatabase.GetCharacterById(client.Value.characterId);
            if (character != null)
            {                
                if (!_player1Spawned)
                {
                    _gameData.ClientIdPlayer1 = client.Value.clientId;
                    Instantiate(character.GameplayPrefab, _player1SpawnLocation.transform)
                        .SpawnAsPlayerObject(client.Value.clientId);
                    _player1Spawned = true;
                }
                else
                {
                    _gameData.ClientIdPlayer2 = client.Value.clientId;
                    Instantiate(character.GameplayPrefab, _player2SpawnLocation.transform)
                        .SpawnAsPlayerObject(client.Value.clientId);
                }
            }
        }
    }
}
