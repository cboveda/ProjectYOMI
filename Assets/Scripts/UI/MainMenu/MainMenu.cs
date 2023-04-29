using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _connectingText;
    [SerializeField] private GameObject _buttonPanel;
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private TMP_InputField _inputField;

    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Player Id: {AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        _connectingText.SetActive(false);
        _buttonPanel.SetActive(true);
    }

    public void StartHost()
    {
        ServerManager.Instance.StartHost();
        _statusText.text = "Starting server...";
    }

    public void StartClient()
    {
        ClientManager.Instance.StartClient(_inputField.text);
        _statusText.text = "Attempting to connect...";
    }
}
