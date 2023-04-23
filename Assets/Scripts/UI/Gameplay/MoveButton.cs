using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _typeText;
    [SerializeField] Button _button;
    private CharacterMove _move;

    public void SetMove(CharacterMove move)
    {
        this._move = move;
        _nameText.text = move.MoveName;
        _typeText.text = Enum.GetName(typeof(CharacterMove.Type), move.MoveType);
        _button.interactable = move.UsableByDefault;

        _button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        GameplayUIManager.Instance.SubmitPlayerAction(_move.Id);
    }
}
