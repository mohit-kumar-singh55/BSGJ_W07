using UnityEngine;

/// <summary>
/// PlayerStateContextクラスは、プレイヤーの状態遷移に必要なコンテキスト情報を保持するクラス
/// </summary>
public class PlayerStateContext
{
    // Cinemachineカメラの切り替えはAnimatorで行う
    private Animator _animator;
    private SpeechDetector _speechDetector;
    private Animator _moeEffectAnimator;
    private VFXCountdown _vfxCountdown;
    private HandDetection _handDetection;
    private Animator _moeExampleAnimator;

    public Animator Animator => _animator;
    public SpeechDetector SpeechDetector => _speechDetector;
    public Animator MoeEffectAnimator => _moeEffectAnimator;
    public VFXCountdown VFXCountdown => _vfxCountdown;
    public HandDetection HandDetection => _handDetection;
    public Animator MoeExampleAnimator => _moeExampleAnimator;

    // player animator params
    public string PAINTING_ANIM { get { return "Painting"; } }

    public PlayerStateContext(Animator animator, SpeechDetector speechDetector, Animator moeEffectAnimator, VFXCountdown vfxCountdown, HandDetection handDetection, Animator moeExampleAnimator)
    {
        _animator = animator;
        _speechDetector = speechDetector;
        _moeEffectAnimator = moeEffectAnimator;
        _vfxCountdown = vfxCountdown;
        _handDetection = handDetection;
        _moeExampleAnimator = moeExampleAnimator;
    }
}