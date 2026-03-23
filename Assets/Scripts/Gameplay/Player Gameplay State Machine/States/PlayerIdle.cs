using UnityEngine;

public class PlayerIdle : BaseState<PlayerStateManager.PlayerState>
{
    private float _idleTimer = 5f;
    private bool _canTransition = false;
    private bool _isFeverMode = false;

    public static event System.Action OnPlayerEnterIdle = delegate { };

    public PlayerIdle(PlayerStateContext context, PlayerStateManager.PlayerState stateKey) : base(stateKey) { }

    public override void EnterState()
    {
        // if fever mode is active, directly transition to moe moe state (no painting in fever mode)
        if (FeverMode.Instance != null && FeverMode.Instance.IsFeverMode)
        {
            _isFeverMode = true;
            return;
        }

        // just wait for sometime and then transition to painting
        _idleTimer = 5f;
        _canTransition = false;

        // spawn food
        OnPlayerEnterIdle?.Invoke();

        // TODO: stop player from painting while in idle
    }

    public override void UpdateState()
    {
        if (_idleTimer <= 0) _canTransition = true;
        else _idleTimer -= Time.deltaTime;
    }

    public override void ExitState()
    {
        // reset
        _isFeverMode = false;
    }

    public override PlayerStateManager.PlayerState GetNextState()
    {
        return _isFeverMode ?
        PlayerStateManager.PlayerState.DoingMoeMoe :
        _canTransition ?
        PlayerStateManager.PlayerState.Painting :
        PlayerStateManager.PlayerState.Idle;
    }
}