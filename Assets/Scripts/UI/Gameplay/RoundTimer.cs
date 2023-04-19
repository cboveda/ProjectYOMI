using TMPro;
using UnityEngine;

public class RoundTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private ProgressBar progressBar;

    [SerializeField] private float maximumTimeSeconds;
    [SerializeField] private float currentTimeSeconds;
    private bool timerActive;

    void Start()
    {
        progressBar.current = 0;
        timerText.text = "0.0";
    }

    void Update()
    {
        if (timerActive)
        {
            UpdateTimer();
            UpdateDisplay();
        }
    }

    void UpdateTimer()
    {
        currentTimeSeconds -= Time.unscaledDeltaTime;
        currentTimeSeconds = Mathf.Clamp(currentTimeSeconds, 0, maximumTimeSeconds);

        if (currentTimeSeconds <= 0f)
        {
            timerActive = false;
            timerText.enabled = false;
        }
    }

    void UpdateDisplay()
    {
        progressBar.current = (int) (currentTimeSeconds * 1000);
        timerText.text = string.Format("{0:0.0}", currentTimeSeconds);
    }

    public void StartTimer(float duration)
    {
        maximumTimeSeconds = duration;
        progressBar.maximum = (int)(maximumTimeSeconds * 1000);
        currentTimeSeconds = maximumTimeSeconds;
        timerActive = true;
        timerText.enabled = true;
    }
}
