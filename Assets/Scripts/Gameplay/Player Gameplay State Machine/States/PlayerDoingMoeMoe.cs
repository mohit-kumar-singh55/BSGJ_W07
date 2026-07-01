using UnityEngine;

/// <summary>
/// MoeMoeステートは、プレイヤーが「もえもえ」アクションを行う状態を管理するクラス
/// </summary>
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

        // subscribe to events
        _context.SpeechDetector.OnRecordingCompleted += OnRecordingCompleted;
        _context.HandDetection.OnHandCheckStart += OnHandCheckStart;
        _context.HandDetection.OnHandDetectionProceed += OnHandDetectionProceed;
        _context.HandDetection.OnHandDetectionOver += OnHandDetectionOver;

        _context.VFXCountdown.StartCountdown(() =>
        {
            // 手認識を開始する
            _context.HandDetection.StartCheck();

            // 萌え萌えの例エフェクトを表示する
            _context.MoeExampleAnimator.gameObject.SetActive(true);
            _context.MoeExampleAnimator.SetBool(_heartPulseAnimHash, true);

            // 萌え萌え アクションが開始されたことを通知する
            OnMoeMoeStarted?.Invoke();
        });
    }

    public override void UpdateState() { }

    public override void ExitState()
    {
        // スコアを保存する
        if (PlayerDataManager.Instance != null)
        {
            float scoreMultiplier = 1f;

            // フィーバーモードのスコア倍率を適用する
            if (FeverMode.Instance != null && FeverMode.Instance.IsFeverMode)
                scoreMultiplier *= FeverMode.Instance.FeverScoreMultiplier;

            // お客の機嫌が悪い場合のスコア倍率を適用する
            if (GlobalData.Instance != null && CustomersManager.Instance.CurrentCustomer.IsInBadMood)
            {
                CustomerMoodSettings customerMoodSettings = GlobalData.Instance.CustomerData.customerMoodSettings;
                if (_totalScore >= customerMoodSettings.minScoreRequiredInBadMood)
                    scoreMultiplier *= customerMoodSettings.badMoodScoreMultiplier;
            }

            // すべてのスコアを合計する
            PlayerDataManager.Instance.AddPlayerScore(Mathf.RoundToInt(_totalScore * scoreMultiplier));
        }
        // フィーバーモードかどうか確認する
        if (FeverMode.Instance != null) FeverMode.Instance.CheckIfPerfect(_totalScore);

        // 現在生成されている料理を削除する
        // 次のお客へ進む
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
        // 萌えのお手本エフェクトを非表示にする
        _context.MoeExampleAnimator.SetBool(_heartPulseAnimHash, false);
        _context.MoeExampleAnimator.gameObject.SetActive(false);

        // 萌えエフェクトを再生する　➀
        _context.MoeEffectAnimator.SetTrigger(MOE1_ANIM);

        // 音声認識を開始する
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
                // 萌えエフェクトを再生する　➁
                _context.MoeEffectAnimator.SetTrigger(MOE2_ANIM);
                break;
            case 4:
                // キュンエフェクトを再生する　➂
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