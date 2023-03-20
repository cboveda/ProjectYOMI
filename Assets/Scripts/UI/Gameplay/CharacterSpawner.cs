using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterSpawner : NetworkBehaviour
{
    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private GameObject player1SpawnLocation;
    [SerializeField] private GameObject player2SpawnLocation;

    private bool player1Spawned = false;

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        foreach (var client in ServerManager.Instance.ClientData)
        {
            var character = characterDatabase.GetCharacterById(client.Value.characterId);
            if (character != null)
            {
                var spawnPos = !player1Spawned ?
                    player1SpawnLocation.transform :
                    player2SpawnLocation.transform;
                if (!player1Spawned)
                {
                    player1Spawned = true;
                }
                var characterInstance = Instantiate(character.GameplayPrefab, spawnPos);
                characterInstance.SpawnAsPlayerObject(client.Value.clientId);

                // Not working... TODO
                //PlayerControls.Instance.RegisterPlayerClientRpc(
                //    character.CharacterMoveSet,
                //    new ClientRpcParams
                //    {
                //        Send = new ClientRpcSendParams
                //        {
                //            TargetClientIds = new ulong[] { client.Value.clientId }
                //        }
                //    }
                //);
            }
        }
    }
}
