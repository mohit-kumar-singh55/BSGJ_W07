using UnityEngine;
using UnityEngine.Rendering;

/*
- フィーバーの条件を一度
    - 連続でタイミングよく萌えキュンできたらフィーバー突入
    - オムライスパートなしで萌えキュンのみできるボーナスタイム
    - 点数が1.2倍で入るように調整
*/
public class FeverMode : Singleton<FeverMode>
{
    [Header("Conditions To Activate Fever Mode")]
    [Tooltip("Number of consecutive perfect 萌えキュン to activate fever mode")]
    [SerializeField] private int _perfectCountToActivateFever = 3;
    [Tooltip("Minimum score to count as perfect 萌えキュン out of 200 (Voice: 100, Hand: 100, not including the Paint)")]
    [SerializeField] private int _minScoreToCountAsPerfect = 120;

    [Space(10)]
    [Header("Fever Mode")]
    [SerializeField] private float _feverDuration = 30f;
    [SerializeField] private float _feverScoreMultiplier = 1.2f;

    [Space(10)]
    [Header("Fever Mode Effects")]
    [SerializeField] private Volume _feverModeLensFlareVolume;

    private bool _isFeverMode = false;
    private int _currentPerfectCount = 0;
    private float _feverTimer = 0f;
    private UIManager _uiManager;
    private AudioManager _audioManager;

    public bool IsFeverMode => _isFeverMode;
    public float FeverScoreMultiplier => _feverScoreMultiplier;

    public static event System.Action OnFeverModeActivated = delegate { };
    public static event System.Action OnFeverModeDeactivated = delegate { };

    private void Start()
    {
        _uiManager = UIManager.Instance;
        _audioManager = AudioManager.Instance;

        if (_uiManager == null)
        {
            Debug.LogError("FeverMode: UIManager is null");
            enabled = false;
            return;
        }

        InitializeData();

        // reset
        ShowFeverModeLensFlare(false);
    }

    private void Update()
    {
        // reset fever mode
        if (_isFeverMode)
        {
            _feverTimer += Time.deltaTime;
            if (_feverTimer >= _feverDuration)
            {
                _isFeverMode = false;
                OnFeverModeDeactivated?.Invoke();
                _currentPerfectCount = 0;
                _feverTimer = 0f;
                _uiManager.UpdateFeverGauge(0f);

                // effects
                ShowFeverModeLensFlare(false);
                _audioManager.PlayBGM(BGM.Mainbgm, 0.5f);   // reset to main bgm
            }
        }
    }

    // ** initialize data from GlobalData **
    private void InitializeData()
    {
        if (GlobalData.Instance == null) return;

        FeverModeSettings fms = GlobalData.Instance.FeverModeData;

        _perfectCountToActivateFever = fms.perfectCountToActivateFever;
        _minScoreToCountAsPerfect = fms.minScoreToCountAsPerfect;
        _feverDuration = fms.feverDuration;
        _feverScoreMultiplier = fms.feverScoreMultiplier;
    }

    public void CheckIfPerfect(int score)
    {
        // return if already in fever mode
        if (_isFeverMode) return;

        // decrement perfect count, if not enough score
        if (score < _minScoreToCountAsPerfect)
            _currentPerfectCount = 0;
        else _currentPerfectCount++;

        // activate fever mode
        if (_currentPerfectCount >= _perfectCountToActivateFever)
        {
            _isFeverMode = true;
            OnFeverModeActivated?.Invoke();

            // effects
            ShowFeverModeLensFlare(true);
            _audioManager.PlayBGM(BGM.FeverMode, 0.5f); // play fever mode bgm
        }

        // update fever gauge ui
        _uiManager.UpdateFeverGauge((float)_currentPerfectCount / _perfectCountToActivateFever);
    }

    private void ShowFeverModeLensFlare(bool show = true) => _feverModeLensFlareVolume.enabled = show;
}