using UnityEngine;

public class PlayerIdle : BaseState<PlayerStateManager.PlayerState>
{
    private float _idleTimer = 3f;
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
        _idleTimer = 3f;
        _canTransition = false;

        OnPlayerEnterIdle?.Invoke();
    }

    public override void UpdateState()
    {
        // stop timer if there is no customer in service, as player cannot paint without customer
        if (CustomersManager.Instance == null || CustomersManager.Instance.CurrentCustomer == null)
            return;

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
        // not allow to transition to painting state if there is no customer in service
        bool customerReady = CustomersManager.Instance != null && CustomersManager.Instance.CurrentCustomer != null;

        return _isFeverMode && customerReady ?
        PlayerStateManager.PlayerState.DoingMoeMoe :
        _canTransition && customerReady ?
        PlayerStateManager.PlayerState.Painting :
        PlayerStateManager.PlayerState.Idle;
    }
}