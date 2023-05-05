using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MoveButton : MonoBehaviour
{
    [SerializeField] TMP_Text _nameText;
    [SerializeField] TMP_Text _typeText;
    [SerializeField] Button _button;
    [SerializeField] private GameObject _highlight;
    private Move _move;
    private IGameUIManager _gameUIManager;
    
    public Button Button { get { return _button; } }

    [Inject]
    public void Construct(IGameUIManager gameUIManager)
    {
        _gameUIManager = gameUIManager;
    }

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
        _gameUIManager.SubmitPlayerAction(_move.Id);
    }

    public void SetHighlight(bool value)
    {
        _highlight.SetActive(value);
    }
}
