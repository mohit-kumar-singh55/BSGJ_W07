using UnityEngine;

public class PlayerDoingMoeMoe : BaseState<PlayerStateManager.PlayerState>
{
    private PlayerStateContext _context;
    private bool _canTransition = false;
    private int _currentPhase = 0;      // 2 = moe1, 3 = moe2, 4 = kyun

    private const string MOE1_ANIM = "Moe1";
    private const string MOE2_ANIM = "Moe2";
    private const string KYUN_ANIM = "Kyun";

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
        _context.HandDetection.OnHandDetectionProceed += OnHandDetectionProceed;
        _context.HandDetection.OnHandDetectionOver += OnHandDetectionOver;

        _context.VFXCountdown.StartCountdown(() =>
        {
            // start hand detection
            _context.HandDetection.StartCheck();
        });
    }

    public override void UpdateState() { }

    public override void ExitState()
    {
        // reset
        _currentPhase = 0;

        // remove current spawned food
        FoodManager.Instance.DestroyFood();
        _context.SpeechDetector.OnRecordingCompleted -= OnRecordingCompleted;
        _context.HandDetection.OnHandCheckStart -= OnHandCheckStart;
        _context.HandDetection.OnHandDetectionProceed -= OnHandDetectionProceed;
        _context.HandDetection.OnHandDetectionOver -= OnHandDetectionOver;
    }

    public override PlayerStateManager.PlayerState GetNextState()
    {
        return _canTransition ? PlayerStateManager.PlayerState.Idle : PlayerStateManager.PlayerState.DoingMoeMoe;
    }

    private void OnHandCheckStart()
    {
        _currentPhase = 2;

        // start moe1 voice detection
        _context.SpeechDetector.StartDetection();

        // play moe effect
        _context.MoeEffectAnimator.SetTrigger(MOE1_ANIM);
    }

    private void OnRecordingCompleted(int score, string message)
    {
        Debug.Log(_currentPhase + ", Score: " + score + ", Message: " + message);
        // if (PlayerDataManager.Instance != null) PlayerDataManager.Instance.AddPlayerScore(score);

        // // play moe effect
        // _context.MoeEffectAnimator.SetTrigger("MoeMoe");

        // _canTransition = true;

        // TODO: add all three scores to the total score
    }

    private void OnHandDetectionProceed(int phase)
    {
        switch (phase)
        {
            case 3:
                _currentPhase = 3;

                // start moe2 voice detection
                // _context.SpeechDetector.StartDetection();

                // play moe effect
                _context.MoeEffectAnimator.SetTrigger(MOE2_ANIM);
                break;
            case 4:
                _currentPhase = 4;

                // start kyun voice detection
                // _context.SpeechDetector.StartDetection();

                // play kyun effect
                _context.MoeEffectAnimator.SetTrigger(KYUN_ANIM);
                break;
        }
    }

    private void OnHandDetectionOver(int point)
    {
        Debug.Log("Point: " + point);
        _canTransition = true;
    }
}