using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text typeText;
    [SerializeField] Button button;

    public void SetMove(CharacterMove move)
    {
        nameText.text = move.MoveName;
        typeText.text = Enum.GetName(typeof(CharacterMove.Type), move.MoveType);
        button.interactable = move.UsableByDefault;
    }
}
