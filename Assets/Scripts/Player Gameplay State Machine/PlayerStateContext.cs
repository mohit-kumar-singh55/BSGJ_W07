using UnityEngine;

public class PlayerStateContext
{
    // cinemachine camera transitions are happening via animator
    private Animator _animator;
    private SpeechDetector _speechDetector;

    public Animator Animator => _animator;
    public SpeechDetector SpeechDetector => _speechDetector;

    public PlayerStateContext(Animator animator, SpeechDetector speechDetector)
    {
        _animator = animator;
        _speechDetector = speechDetector;
    }
}