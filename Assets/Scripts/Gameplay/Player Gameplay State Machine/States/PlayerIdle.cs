using UnityEngine;

/// <summary>
/// プレイヤーの待機状態を表すステート
/// 待機状態では、プレイヤーは何もせず、一定時間経過後にペイントステートに遷移する
/// ただし、フィーバーモード中はペイントステートに遷移しない
/// </summary>
public class PlayerIdle : BaseState<PlayerStateManager.PlayerState>
{
    private float _idleTimer = 3f;
    private bool _canTransition = false;
    private bool _isFeverMode = false;

    public static event System.Action OnPlayerEnterIdle = delegate { };

    public PlayerIdle(PlayerStateContext context, PlayerStateManager.PlayerState stateKey) : base(stateKey) { }

    public override void EnterState()
    {
        // フィーバーモード中なら、そのままMoeMoeステートへ遷移する
        // （フィーバーモード中はお絵描きパートなし）
        if (FeverMode.Instance != null && FeverMode.Instance.IsFeverMode)
        {
            _isFeverMode = true;
            return;
        }

        // 少し待機してからPaintingステートへ遷移する
        _idleTimer = 3f;
        _canTransition = false;

        OnPlayerEnterIdle?.Invoke();
    }

    public override void UpdateState()
    {
        // 接客中のお客がいない場合はタイマーを停止する
        // （お客がいないとお絵描きできないため）
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
        // 接客中のお客がいない場合はPaintingステートへ遷移させない
        bool customerReady = CustomersManager.Instance != null && CustomersManager.Instance.CurrentCustomer != null;

        return _isFeverMode && customerReady ?
        PlayerStateManager.PlayerState.DoingMoeMoe :
        _canTransition && customerReady ?
        PlayerStateManager.PlayerState.Painting :
        PlayerStateManager.PlayerState.Idle;
    }
}