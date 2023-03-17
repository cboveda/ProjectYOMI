using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    public void Exit()
    {
        ServerManager.Instance.Disconnect();
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
