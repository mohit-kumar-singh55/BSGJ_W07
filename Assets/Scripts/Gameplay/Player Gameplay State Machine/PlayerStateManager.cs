using UnityEngine;

/// <summary>
/// プレイヤーの状態を管理するステートマシン
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerStateManager : StateManager<PlayerStateManager.PlayerState>
{
    public enum PlayerState { Idle, Painting, DoingMoeMoe }

    [Header("Speech Detection")]
    [SerializeField] private SpeechDetector _speechDetector;

    [Space(10)]
    [Header("Hand Detection")]
    [SerializeField] private HandDetection _handDetection;

    [Space(10)]
    [Header("Animation")]
    [SerializeField] private Animator _moeEffectAnimator;
    [SerializeField] private Animator _moeExampleAnimator;

    [Space(10)]
    [Header("MISC")]
    [SerializeField] private VFXCountdown _vfxCountdown;

    private PlayerStateContext _context;

    void OnEnable()
    {
        // 時間切れになったらプレイヤーのステートマシンを停止する
        Timer.OnTimesUp += OnTimesUp;
    }

    void OnDisable()
    {
        Timer.OnTimesUp -= OnTimesUp;
    }

    void Awake()
    {
        _context = new PlayerStateContext(GetComponent<Animator>(), _speechDetector, _moeEffectAnimator, _vfxCountdown, _handDetection, _moeExampleAnimator);
        InitializeStates();
    }

    private void InitializeStates()
    {
        // 継承元のStateManagerのStates辞書にステートを追加し、初期ステートを設定する
        States.Add(PlayerState.Idle, new PlayerIdle(_context, PlayerState.Idle));
        States.Add(PlayerState.Painting, new PlayerPainting(_context, PlayerState.Painting));
        States.Add(PlayerState.DoingMoeMoe, new PlayerDoingMoeMoe(_context, PlayerState.DoingMoeMoe));

        // 常にIdleステートから開始する
        CurrentState = States[PlayerState.Idle];
    }

    private void OnTimesUp()
    {
        CurrentState.ExitState();
        gameObject.SetActive(false);
    }
}