using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    private CharacterSelectDisplay characterSelect;
    private Character character; 
    public void SetCharacter(CharacterSelectDisplay charcterSelect, Character character)
    {
        iconImage.sprite = character.Icon;

        this.characterSelect = charcterSelect;
        this.character = character;
    }

    public void SelectCharacter()
    {
        characterSelect.Select(character);
    }
}
