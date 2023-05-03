using UnityEngine;
using Zenject;

public class OptionsMenu : MonoBehaviour
{
    private IServerManager _serverManager;

    [Inject]
    public void Construct(IServerManager serverManager)
    {
        _serverManager = serverManager;
    }

    public void Exit()
    {
        _serverManager.Disconnect();
        Application.Quit();
#if (UNITY_EDITOR)
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
