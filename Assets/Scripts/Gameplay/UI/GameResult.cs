using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Zenject;

public class GameResult : MonoBehaviour
{
    [SerializeField] private TMP_Text _result;
    [SerializeField] private GameObject _restartButton;
    [SerializeField] private GameObject _restartText;
    public string Result { set { _result.text = value; } }
    public GameObject RestartButton { get { return _restartButton; } }
    public GameObject RestartText { get { return _restartText; } }

    private IServerManager _serverManager;
    private NetworkManager _networkManager;

    [Inject]
    public void Construct(NetworkManager networkManager, IServerManager serverManager)
    {
        _serverManager = serverManager;
        _networkManager = networkManager;
    }

    public void Reset()
    {
        if (!_networkManager.IsServer) return;
        _serverManager.ResetGame();
    }
}
