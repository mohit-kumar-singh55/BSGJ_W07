using UnityEngine;

public class PlayerStateContext
{
    // cinemachine camera transitions are happening via animator
    private Animator _animator;
    private SpeechDetector _speechDetector;
    private Animator _moeEffectAnimator;
    private VFXCountdown _vfxCountdown;

    public Animator Animator => _animator;
    public SpeechDetector SpeechDetector => _speechDetector;
    public Animator MoeEffectAnimator => _moeEffectAnimator;
    public VFXCountdown VFXCountdown => _vfxCountdown;

    public PlayerStateContext(Animator animator, SpeechDetector speechDetector, Animator moeEffectAnimator, VFXCountdown vfxCountdown)
    {
        _animator = animator;
        _speechDetector = speechDetector;
        _moeEffectAnimator = moeEffectAnimator;
        _vfxCountdown = vfxCountdown;
    }
}