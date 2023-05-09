using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Button _button;
    [SerializeField] private Image _borderImage;

    private CharacterSelectDisplay _characterSelect;
    public Character Character { get; private set; }

    public bool IsEnabled { get { return _button.interactable; } private set { _button.interactable = value; } }
    public bool IsSelected { get; private set; }
    public void SetCharacter(CharacterSelectDisplay characterSelect, Character character)
    {
        _iconImage.sprite = character.Icon;
        _characterSelect = characterSelect;
        Character = character;
        IsSelected = false;
    }

    public void SelectCharacter()
    {
        _characterSelect.Select(Character);
    }

    public void ShowSelected()
    {
        IsSelected = true;
        _borderImage.color = Color.cyan;
    }

    public void ShowUnselected()
    {
        IsSelected = false;
        _borderImage.color = Color.black;
    }

    public void SetDisabled()
    {
        IsEnabled = false;
    }

    public void SetEnabled()
    {
        IsEnabled = true;
    }
}
