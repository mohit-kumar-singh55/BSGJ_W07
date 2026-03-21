using UnityEngine;

public class PlayerDoingMoeMoe : BaseState<PlayerStateManager.PlayerState>
{
    private PlayerStateContext _context;
    private bool _canTransition = false;

    public PlayerDoingMoeMoe(PlayerStateContext context, PlayerStateManager.PlayerState stateKey) : base(stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        _canTransition = false;
        // TODO: wait for some seconds, play some sound

        // events
        _context.SpeechDetector.OnRecordingCompleted += OnRecordingCompleted;
        _context.HandDetection.OnHandCheckStart += OnHandCheckStart;
        _context.HandDetection.OnHandDetectionOver += OnHandDetectionOver;

        _context.VFXCountdown.StartCountdown(() =>
        {
            // start hand detection
            _context.HandDetection.StartCheck();
        });

        // TODO: check hand gesture
        // TODO: add events to capture them and once done, call another function that will check the current score and the time remaining, if time remaining, transition to idle, if not, go to next scene
    }

    public override void UpdateState() { }

    public override void ExitState()
    {
        // remove current spawned food
        FoodManager.Instance.DestroyFood();
        _context.SpeechDetector.OnRecordingCompleted -= OnRecordingCompleted;
        _context.HandDetection.OnHandCheckStart -= OnHandCheckStart;
        _context.HandDetection.OnHandDetectionOver -= OnHandDetectionOver;
    }

    public override PlayerStateManager.PlayerState GetNextState()
    {
        return _canTransition ? PlayerStateManager.PlayerState.Idle : PlayerStateManager.PlayerState.DoingMoeMoe;
    }

    private void OnHandCheckStart()
    {
        // start voice detection
        _context.SpeechDetector.StartDetection();

        // play moe effect
        _context.MoeEffectAnimator.SetTrigger("MoeMoe");
    }

    private void OnRecordingCompleted(int score, string message)
    {
        if (PlayerDataManager.Instance != null) PlayerDataManager.Instance.AddPlayerScore(score);

        // // play moe effect
        // _context.MoeEffectAnimator.SetTrigger("MoeMoe");

        // _canTransition = true;

        // TODO: add all three scores to the total score
    }

    private void OnHandDetectionOver(int point)
    {
        Debug.Log("Point: " + point);
    }
}