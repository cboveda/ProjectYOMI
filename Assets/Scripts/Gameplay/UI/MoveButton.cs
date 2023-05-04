using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _typeText;
    [SerializeField] Button _button;
    private Move _move;
    [SerializeField] private GameObject _highlight;

    public Button Button { get { return _button; } }

    public void SetMove(Move move)
    {
        this._move = move;
        _typeText.text = move.MoveName;
        _nameText.text = Enum.GetName(typeof(Move.Type), move.MoveType);
        _button.interactable = move.UsableByDefault;

        _button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        GameUIManager.Instance.SubmitPlayerAction(_move.Id);
    }

    public void SetHighlight(bool value)
    {
        _highlight.SetActive(value);
    }
}
