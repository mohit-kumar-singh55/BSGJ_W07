using UnityEngine;

public class PlayerDoingMoeMoe : BaseState<PlayerStateManager.PlayerState>
{
    private PlayerStateContext _context;
    private int _totalScore = 0;        // voice + hand
    private bool _voiceDetectionFinished = false;
    private bool _handDetectionFinished = false;
    private int _heartPulseAnimHash;

    private const string MOE1_ANIM = "Moe1";
    private const string MOE2_ANIM = "Moe2";
    private const string KYUN_ANIM = "Kyun";
    private const string HEART_PULSE = "Pulse";

    public static event System.Action OnMoeMoeStarted = delegate { };
    public static event System.Action OnMoeMoeCompleted = delegate { };

    public PlayerDoingMoeMoe(PlayerStateContext context, PlayerStateManager.PlayerState stateKey) : base(stateKey)
    {
        _context = context;
        _heartPulseAnimHash = Animator.StringToHash(HEART_PULSE);
    }

    public override void EnterState()
    {
        // reset
        _totalScore = 0;
        _voiceDetectionFinished = false;
        _handDetectionFinished = false;

        // TODO: wait for some seconds, play some sound

        // subscribe to events
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
            _context.MoeExampleAnimator.SetBool(_heartPulseAnimHash, true);

            // moe moe started
            OnMoeMoeStarted?.Invoke();
        });
    }

    public override void UpdateState() { }

    public override void ExitState()
    {
        // save score
        if (PlayerDataManager.Instance != null)
        {
            float scoreMultiplier = 1f;

            // apply fever score multiplier
            if (FeverMode.Instance != null && FeverMode.Instance.IsFeverMode)
                scoreMultiplier *= FeverMode.Instance.FeverScoreMultiplier;

            // apply customer's bad mood score multiplier
            if (GlobalData.Instance != null && CustomersManager.Instance.CurrentCustomer.IsInBadMood)
            {
                CustomerMoodSettings customerMoodSettings = GlobalData.Instance.CustomerData.customerMoodSettings;
                if (_totalScore >= customerMoodSettings.minScoreRequiredInBadMood)
                    scoreMultiplier *= customerMoodSettings.badMoodScoreMultiplier;
            }

            // addup all scores
            PlayerDataManager.Instance.AddPlayerScore(Mathf.RoundToInt(_totalScore * scoreMultiplier));
        }
        // check for fever mode
        if (FeverMode.Instance != null) FeverMode.Instance.CheckIfPerfect(_totalScore);

        // remove current spawned food
        // go to next customer
        OnMoeMoeCompleted?.Invoke();

        // unsubscribe from events
        _context.SpeechDetector.OnRecordingCompleted -= OnRecordingCompleted;
        _context.HandDetection.OnHandCheckStart -= OnHandCheckStart;
        _context.HandDetection.OnHandDetectionProceed -= OnHandDetectionProceed;
        _context.HandDetection.OnHandDetectionOver -= OnHandDetectionOver;

        // playing sfx
        if (AudioManager.Instance != null)
        {
            if (FeverMode.Instance != null && FeverMode.Instance.IsFeverMode)
                AudioManager.Instance.PlaySFX(SFX.FeverScoreUp);
            else
                // AudioManager.Instance.PlaySFX(SFX.ScoreUp);
                AudioManager.Instance.PlaySFX(SFX.Itterasshai);
        }
    }

    public override PlayerStateManager.PlayerState GetNextState()
    {
        return (_voiceDetectionFinished && _handDetectionFinished) ? PlayerStateManager.PlayerState.Idle : PlayerStateManager.PlayerState.DoingMoeMoe;
    }

    private void OnHandCheckStart()
    {
        // hide moe example effect
        _context.MoeExampleAnimator.SetBool(_heartPulseAnimHash, false);
        _context.MoeExampleAnimator.gameObject.SetActive(false);

        // play moe effect
        _context.MoeEffectAnimator.SetTrigger(MOE1_ANIM);

        // start voice detection
        _context.SpeechDetector.StartDetection();
    }

    private void OnRecordingCompleted(int score, string message)
    {
        _totalScore += score;
        _voiceDetectionFinished = true;
    }

    private void OnHandDetectionProceed(int phase)
    {
        switch (phase)
        {
            case 3:
                // play moe effect
                _context.MoeEffectAnimator.SetTrigger(MOE2_ANIM);
                break;
            case 4:
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

        _totalScore += handScore;
        _handDetectionFinished = true;
    }
}