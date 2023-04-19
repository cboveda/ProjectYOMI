using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterSpawner : NetworkBehaviour
{
    [SerializeField] private CharacterDatabase _characterDatabase;
    [SerializeField] private GameObject _player1SpawnLocation;
    [SerializeField] private GameObject _player2SpawnLocation;

    private bool player1Spawned = false;

    public void Start()
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
                var spawnPos = !player1Spawned ?
                    _player1SpawnLocation.transform :
                    _player2SpawnLocation.transform;
                if (!player1Spawned)
                {
                    player1Spawned = true;
                }
                var characterInstance = Instantiate(character.GameplayPrefab, spawnPos);
                characterInstance.SpawnAsPlayerObject(client.Value.clientId);
            }
        }
    }
}
