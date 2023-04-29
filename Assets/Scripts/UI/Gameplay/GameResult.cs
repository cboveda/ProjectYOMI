using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class GameResult : MonoBehaviour
{
    [SerializeField] private TMP_Text _result;
    [SerializeField] private GameObject _restartButton;
    [SerializeField] private GameObject _restartText;
    public string Result { set { _result.text = value; } }
    public GameObject RestartButton { get { return _restartButton; } }
    public GameObject RestartText { get { return _restartText; } }

    public void Reset()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        ServerManager.Instance.ResetGame();
    }
}
