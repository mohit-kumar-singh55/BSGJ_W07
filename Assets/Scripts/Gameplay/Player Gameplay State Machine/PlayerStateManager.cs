using UnityEngine;

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
    [SerializeField] private VFXCountdown _vfxCountdown;    // TODO: make this to be always in front and center of the camera like billboard

    private PlayerStateContext _context;

    void OnEnable()
    {
        // stop player state machine, when times up
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
        // Add states to inherited StateManager "States" dictionary and set Initial State
        States.Add(PlayerState.Idle, new PlayerIdle(_context, PlayerState.Idle));
        States.Add(PlayerState.Painting, new PlayerPainting(_context, PlayerState.Painting));
        States.Add(PlayerState.DoingMoeMoe, new PlayerDoingMoeMoe(_context, PlayerState.DoingMoeMoe));

        // always start in idle
        CurrentState = States[PlayerState.Idle];
    }

    private void OnTimesUp()
    {
        CurrentState.ExitState();
        gameObject.SetActive(false);
    }
}