using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Button _button;
    [SerializeField] private Image _borderImage;

    private CharacterSelectDisplay _characterSelect;
    public Character Character { get; private set; }

    public bool IsDisabled { get; private set; }
    public void SetCharacter(CharacterSelectDisplay characterSelect, Character character)
    {
        _iconImage.sprite = character.Icon;
        this._characterSelect = characterSelect;
        Character = character;
    }

    public void SelectCharacter()
    {
        _characterSelect.Select(Character);
    }

    public void ShowSelected()
    {
        _borderImage.color = Color.cyan;
    }

    public void ShowUnselected()
    {
        _borderImage.color = Color.black;
    }

    public void SetDisabled()
    {
        _button.interactable = false;
    }
}
