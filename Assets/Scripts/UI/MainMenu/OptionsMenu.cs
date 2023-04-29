using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public void Exit()
    {
        ServerManager.Instance.Disconnect();
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
