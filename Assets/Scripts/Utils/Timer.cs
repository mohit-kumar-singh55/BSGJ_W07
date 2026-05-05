using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float _timeLimit = 90f;

    private float _currentTime;
    private bool _isRunning = true;
    private UIManager _uiManager;

    public float TimeLimit => _timeLimit;
    public float CurrentTime => _currentTime;

    public static event System.Action OnTimesUp = delegate { };

    private void OnEnable()
    {
        FeverMode.OnFeverModeActivated += PauseTimer;
        FeverMode.OnFeverModeDeactivated += ResumeTimer;
    }

    private void OnDisable()
    {
        FeverMode.OnFeverModeActivated -= PauseTimer;
        FeverMode.OnFeverModeDeactivated -= ResumeTimer;
    }

    private void Start()
    {
        // initialize data
        InitializeData();

        _currentTime = _timeLimit;
        _uiManager = UIManager.Instance;
    }

    // ** タイマーの更新 **
    private void Update()
    {
        if (!_isRunning) return;

        _currentTime -= Time.deltaTime;

        if (_currentTime <= 0)
        {
            _currentTime = 0;
            _isRunning = false;

            // stop player state machine
            // go to next scene
            OnTimesUp?.Invoke();
            return;
        }

        // int minutes = Mathf.FloorToInt(_currentTime / 60);
        // int seconds = Mathf.FloorToInt(_currentTime % 60);

        // UI更新
        // _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        _uiManager.RotateNeedle(_currentTime / _timeLimit);
    }

    // ** initialize data from GlobalData **
    private void InitializeData()
    {
        if (GlobalData.Instance == null) return;

        TimerSettings cs = GlobalData.Instance.TimerData;

        _timeLimit = cs.timeLimit;
    }

    private void PauseTimer() => _isRunning = false;

    private void ResumeTimer() => _isRunning = true;
}