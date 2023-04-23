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
                var spawnPos = !_player1Spawned ?
                    _player1SpawnLocation.transform :
                    _player2SpawnLocation.transform;
                var characterInstance = Instantiate(character.GameplayPrefab, spawnPos);
                characterInstance.SpawnAsPlayerObject(client.Value.clientId);
                if (!_player1Spawned)
                {
                    _gameData.ClientIdPlayer1 = client.Value.clientId;
                    _player1Spawned = true;
                }
                else
                {
                    _gameData.ClientIdPlayer2 = client.Value.clientId;
                }
            }
        }
    }
}
