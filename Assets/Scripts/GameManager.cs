using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{



    #region DevelopmentUI
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 140, 150, 300));
        if (NetworkManager.Singleton.IsClient)
        {
            PlayerActionButtons();
            PlayerStatusLabels();
        }
        GUILayout.EndArea();
    }

    void PlayerStatusLabels()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();

            GUILayout.Label("Round Timer: ");
            GUILayout.Label("Next move: " + Enum.GetName(typeof(Player.PlayerActions), player.PlayerAction.Value));
        }
    }

    void PlayerActionButtons()
    {
        var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        if (GUILayout.Button("Light Attack")) { player.LightAttack(); }
        if (GUILayout.Button("Heavy Attack")) { player.HeavyAttack(); }
        if (GUILayout.Button("Parry")) { player.Parry(); }
        if (GUILayout.Button("Grab")) { player.Grab(); }
        if (GUILayout.Button("Health1")) { player.SetPlayerHealthServerRpc(1, 100); }
        if (GUILayout.Button("Health2")) { player.SetPlayerHealthServerRpc(2, 99); }
    }
    #endregion



}
