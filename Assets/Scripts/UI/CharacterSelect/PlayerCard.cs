using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private CharacterDatabase _characterDatabase;
    [SerializeField] private GameObject _visuals;
    [SerializeField] private Image _characterIconImage;
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private TMP_Text _playerStatusText;

    public void UpdateDisplay(CharacterSelectState state)
    {
        var character = _characterDatabase.GetCharacterById(state.CharacterId);

        if (state.CharacterId != -1)
        {
            _characterIconImage.sprite = character.Icon;
        }

        _playerNameText.text = $"Player {state.ClientId}";
        _playerStatusText.text = state.IsLockedIn ? "Ready!" : "Picking character...";
        _visuals.SetActive(true);
    }

    public void DisableDisplay()
    {
        _visuals.SetActive(false);
    }

    public void EnableIcon()
    {
        _characterIconImage.enabled = true;
    }
}
