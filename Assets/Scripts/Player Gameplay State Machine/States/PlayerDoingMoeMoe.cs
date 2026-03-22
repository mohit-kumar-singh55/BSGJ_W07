using UnityEngine;

public class PlayerDoingMoeMoe : BaseState<PlayerStateManager.PlayerState>
{
    private PlayerStateContext _context;
    private int _currentPhase = 0;      // 2 = moe1, 3 = moe2, 4 = kyun
    private bool _voiceDetectionFinished = false;
    private bool _handDetectionFinished = false;

    private const string MOE1_ANIM = "Moe1";
    private const string MOE2_ANIM = "Moe2";
    private const string KYUN_ANIM = "Kyun";

    public PlayerDoingMoeMoe(PlayerStateContext context, PlayerStateManager.PlayerState stateKey) : base(stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        _voiceDetectionFinished = false;
        _handDetectionFinished = false;

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

            // show moe example effect
            _context.MoeExampleAnimator.gameObject.SetActive(true);
            _context.MoeExampleAnimator.SetBool("Pulse", true);
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
        // return (_voiceDetectionFinished && _handDetectionFinished) ? PlayerStateManager.PlayerState.Idle : PlayerStateManager.PlayerState.DoingMoeMoe;
        return _handDetectionFinished ? PlayerStateManager.PlayerState.Idle : PlayerStateManager.PlayerState.DoingMoeMoe;
    }

    private void OnHandCheckStart()
    {
        _currentPhase = 2;

        // hide moe example effect
        _context.MoeExampleAnimator.SetBool("Pulse", false);
        _context.MoeExampleAnimator.gameObject.SetActive(false);

        // play moe effect
        _context.MoeEffectAnimator.SetTrigger(MOE1_ANIM);

        // start voice detection
        // _context.SpeechDetector.StartDetection();
    }

    private void OnRecordingCompleted(int score, string message)
    {
        Debug.Log(_currentPhase + ", Score: " + score + ", Message: " + message);
        if (PlayerDataManager.Instance != null) PlayerDataManager.Instance.AddPlayerScore(score);
        _voiceDetectionFinished = true;
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
        int handScore = point switch
        {
            1 => 50,
            2 => 75,
            3 => 100,
            _ => 25,
        };

        Debug.Log("Hand Detection Over, Score: " + handScore);

        if (PlayerDataManager.Instance != null) PlayerDataManager.Instance.AddPlayerScore(handScore);
        _handDetectionFinished = true;
    }
}