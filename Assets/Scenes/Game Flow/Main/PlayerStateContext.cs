using UnityEngine;

public class PlayerStateContext
{
    // cinemachine camera transitions are happening via animator
    private Animator _animator;

    public Animator Animator => _animator;

    public PlayerStateContext(Animator animator) => _animator = animator;
}