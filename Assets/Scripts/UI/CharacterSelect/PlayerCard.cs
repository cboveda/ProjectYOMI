using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private GameObject visuals;
    [SerializeField] private Image characterIconImage;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerStatusText;

    public void UpdateDisplay(CharacterSelectState state)
    {
        var character = characterDatabase.GetCharacterById(state.CharacterId);

        if (state.CharacterId != -1)
        {
            characterIconImage.sprite = character.Icon;
        }

        playerNameText.text = $"Player {state.ClientId}";
        playerStatusText.text = state.IsLockedIn ? "Ready!" : "Picking character...";
        visuals.SetActive(true);
    }

    public void DisableDisplay()
    {
        visuals.SetActive(false);
    }

    public void EnableIcon()
    {
        characterIconImage.enabled = true;
    }
}
