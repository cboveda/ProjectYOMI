using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;
    [SerializeField] private Image borderImage;

    private CharacterSelectDisplay characterSelect;
    public Character Character { get; private set; }

    public bool IsDisabled { get; private set; }
    public void SetCharacter(CharacterSelectDisplay characterSelect, Character character)
    {
        iconImage.sprite = character.Icon;
        this.characterSelect = characterSelect;
        Character = character;
    }

    public void SelectCharacter()
    {
        characterSelect.Select(Character);
    }

    public void ShowSelected()
    {
        borderImage.color = Color.cyan;
    }

    public void ShowUnselected()
    {
        borderImage.color = Color.black;
    }

    public void SetDisabled()
    {
        button.interactable = false;
    }
}
