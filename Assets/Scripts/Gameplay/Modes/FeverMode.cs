using UnityEngine;

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
    [Tooltip("Minimum score to count as perfect 萌えキュン out of 200 (Vocie: 100, Hand: 100, not including the Paint)")]
    [SerializeField] private int _minScoreToCountAsPerfect = 120;

    [Space(10)]
    [Header("Fever Mode")]
    [SerializeField] private float _feverDuration = 30f;
    [SerializeField] private float _feverScoreMultiplier = 1.2f;

    private bool _isFeverMode = false;
    private int _currentPerfectCount = 0;
    private float _feverTimer = 0f;
    // private bool __wasLastOnePerfect = false;
    private UIManager _uiManager;

    public bool IsFeverMode => _isFeverMode;
    public float FeverScoreMultiplier => _feverScoreMultiplier;

    void Start()
    {
        _uiManager = UIManager.Instance;

        if (_uiManager == null)
        {
            Debug.LogError("FeverMode: UIManager is null");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        // reset fever mode
        if (_isFeverMode)
        {
            _feverTimer += Time.deltaTime;
            if (_feverTimer >= _feverDuration)
            {
                _isFeverMode = false;
                _currentPerfectCount = 0;
                _feverTimer = 0f;
                _uiManager.UpdateFeverGauge(0f);
                // ! temp
                _uiManager.UpdateFeverGaugeText(false);
            }
        }
    }

    public void CheckIfPerfect(int score)
    {
        Debug.Log("************* Score: " + score + " *************");

        // return if already in fever mode
        if (_isFeverMode) return;

        // decrement perfect count, if not enough score
        if (score < _minScoreToCountAsPerfect)
        {
            _currentPerfectCount = 0;
            // __wasLastOnePerfect = false;
            // return;
        }
        else _currentPerfectCount++;

        // 
        // if (_currentPerfectCount <= 0 || __wasLastOnePerfect)
        //     _currentPerfectCount++;
        // __wasLastOnePerfect = true;

        // activate fever mode
        if (_currentPerfectCount >= _perfectCountToActivateFever)
        {
            _isFeverMode = true;
            // Debug.Log("Fever Mode Activated!");
            // ! temp
            _uiManager.UpdateFeverGaugeText(true);
        }

        // update fever gauge ui
        _uiManager.UpdateFeverGauge((float)_currentPerfectCount / _perfectCountToActivateFever);
    }
}