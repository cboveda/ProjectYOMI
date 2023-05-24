using ModestTree;
using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Zenject;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _connectingText;
    [SerializeField] private GameObject _buttonPanel;
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private TMP_InputField _inputField;

    private IServerManager _serverManager;
    private IClientManager _clientManager;

    [Inject]
    public void Construct(IServerManager serverManager, IClientManager clientManager)
    {
        _serverManager = serverManager;
        _clientManager = clientManager;
    }

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
        _inputField.onSubmit.AddListener((e) =>
        {
            StartClient();
            FocusInputField();
        });
        FocusInputField();
    }

    public void StartHost()
    {
        _serverManager.StartHost();
        _statusText.text = "Starting server...";
    }

    public void StartClient()
    {
        var joinCode = _inputField.text;
        if (joinCode.Length == 0)
        {
            _statusText.text = "Please enter a join code.";
            return;
        }
        if (joinCode.Length != 6)
        {
            _statusText.text = "Please enter a valid join code.";
            return;
        }
        _clientManager.StartClient(joinCode, (message) => _statusText.text = message);

    }

    private void FocusInputField()
    {
        _inputField.Select();
        _inputField.ActivateInputField();
    }
}
