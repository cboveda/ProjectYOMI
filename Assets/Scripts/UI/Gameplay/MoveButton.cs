using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text typeText;
    [SerializeField] Button button;
    private CharacterMove move;

    public void SetMove(CharacterMove move)
    {
        this.move = move;
        nameText.text = move.MoveName;
        typeText.text = Enum.GetName(typeof(CharacterMove.Type), move.MoveType);
        button.interactable = move.UsableByDefault;

        button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerCharacter>().SubmitPlayerActionServerRpc(move.Id);
    }
}
