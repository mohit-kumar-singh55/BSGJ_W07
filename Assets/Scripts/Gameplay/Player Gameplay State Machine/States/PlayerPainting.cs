/// <summary>
/// プレイヤーのペイント状態を表すステート
/// ペイント状態では、プレイヤーはお絵描きパートを行い、描き終わったらMoeMoeステートに遷移する
/// </summary>
public class PlayerPainting : BaseState<PlayerStateManager.PlayerState>
{
    private PlayerStateContext _context;
    private bool _canTransition = false;

    public static event System.Action OnPlayerEnterPainting = delegate { };
    public static event System.Action OnPlayerExitPainting = delegate { };

    public PlayerPainting(PlayerStateContext context, PlayerStateManager.PlayerState stateKey) : base(stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        _canTransition = false;
        OnPlayerEnterPainting?.Invoke();

        // カメラ切り替え
        _context.Animator.SetBool(_context.PAINTING_ANIM, true);

        // events 
        KetchupPainter.OnFinishedDrawing += OnDrawingFinished;
    }

    public override void UpdateState() { }

    public override void ExitState()
    {
        OnPlayerExitPainting?.Invoke();

        // カメラ切り替え
        _context.Animator.SetBool(_context.PAINTING_ANIM, false);

        // unsubscribe
        KetchupPainter.OnFinishedDrawing -= OnDrawingFinished;
    }

    public override PlayerStateManager.PlayerState GetNextState()
    {
        return _canTransition ? PlayerStateManager.PlayerState.DoingMoeMoe : PlayerStateManager.PlayerState.Painting;
    }

    private void OnDrawingFinished(int score)
    {
        if (PlayerDataManager.Instance != null) PlayerDataManager.Instance.AddPlayerScore(score);
        _canTransition = true;
    }
}