public class PlayerPainting : BaseState<PlayerStateManager.PlayerState>
{
    private PlayerStateContext _context;

    public PlayerPainting(PlayerStateContext context, PlayerStateManager.PlayerState stateKey) : base(stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        // set animator bool
        // wait for paiting to complete (add the event in painting class, if no strokes left, then call the event here and transition to doing moe moe)
        _context.Animator.SetBool("Painting", true);
    }

    public override void UpdateState() { }

    public override void ExitState()
    {
        _context.Animator.SetBool("Painting", false);
    }

    public override PlayerStateManager.PlayerState GetNextState()
    {
        // return PlayerStateManager.PlayerState.DoingMoeMoe;
        return PlayerStateManager.PlayerState.Painting;
    }
}