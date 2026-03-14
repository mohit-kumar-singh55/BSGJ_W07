public class PlayerDoingMoeMoe : BaseState<PlayerStateManager.PlayerState>
{
    private PlayerStateContext _context;
    private bool _canTransition = false;

    public PlayerDoingMoeMoe(PlayerStateContext context, PlayerStateManager.PlayerState stateKey) : base(stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        _canTransition = false;
        // TODO: wait for some seconds and play moe moe kyun effect
        // check voice
        _context.SpeechDetector.OnRecordingCompleted += OnRecordingCompleted;
        _context.SpeechDetector.StartDetection();

        // TODO: check hand gesture
        // TODO: add events to capture them and once done, call another function that will check the current score and the time remaining, if time remaining, transition to idle, if not, go to next scene
    }

    public override void UpdateState() { }

    public override void ExitState()
    {
        // remove current spawned food
        FoodManager.Instance.DestroyFood();
        _context.SpeechDetector.OnRecordingCompleted -= OnRecordingCompleted;
    }

    public override PlayerStateManager.PlayerState GetNextState()
    {
        return _canTransition ? PlayerStateManager.PlayerState.Idle : PlayerStateManager.PlayerState.DoingMoeMoe;
    }

    public void OnRecordingCompleted(int score, string message)
    {
        if (PlayerDataManager.Instance != null) PlayerDataManager.Instance.AddPlayerScore(score);
        _canTransition = true;

        // TODO: add all three scores to the total score
    }
}