public class PlayerPainting : BaseState<PlayerStateManager.PlayerState>
{
    private PlayerStateContext _context;
    private bool _canTransition = false;

    public PlayerPainting(PlayerStateContext context, PlayerStateManager.PlayerState stateKey) : base(stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        _canTransition = false;
        _context.Animator.SetBool("Painting", true);
        KetchupPainter.OnFinishedDrawing += OnDrawingFinished;
    }

    public override void UpdateState() { }

    public override void ExitState()
    {
        _context.Animator.SetBool("Painting", false);
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