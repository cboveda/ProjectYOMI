using TMPro;
using UnityEngine;

public class RoundTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private ProgressBar _progressBar;

    [SerializeField] private float _maximumTimeSeconds;
    [SerializeField] private float _currentTimeSeconds;
    private bool _timerActive;

#if UNITY_INCLUDE_TESTS
    public float CurrentTimeSeconds { get => _currentTimeSeconds; }
    public bool TimerActive { get => _timerActive; }
#endif


    void Start()
    {
        _progressBar.current = 0;
        _timerText.text = "0.0";
    }

    void Update()
    {
        if (_timerActive)
        {
            UpdateTimer();
            UpdateDisplay();
        }
    }

    void UpdateTimer()
    {
        _currentTimeSeconds -= Time.unscaledDeltaTime;
        _currentTimeSeconds = Mathf.Clamp(_currentTimeSeconds, 0, _maximumTimeSeconds);

        if (_currentTimeSeconds <= 0f)
        {
            _timerActive = false;
            _timerText.enabled = false;
            _progressBar.gameObject.SetActive(false);
        }
    }

    void UpdateDisplay()
    {
        _progressBar.SetCurrent(_currentTimeSeconds * 1000);
        _timerText.text = string.Format("{0:0.0}", _currentTimeSeconds);
    }

    public void StartTimer(float duration)
    {
        _maximumTimeSeconds = duration;
        _progressBar.SetMaximum(_maximumTimeSeconds * 1000);
        _currentTimeSeconds = _maximumTimeSeconds;
        _timerActive = true;
        _timerText.enabled = true;
        _progressBar.gameObject.SetActive(true);

    }
}
