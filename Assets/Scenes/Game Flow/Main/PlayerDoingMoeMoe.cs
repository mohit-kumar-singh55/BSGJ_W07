public class PlayerDoingMoeMoe : BaseState<PlayerStateManager.PlayerState>
{
    public PlayerDoingMoeMoe(PlayerStateContext context, PlayerStateManager.PlayerState stateKey) : base(stateKey) { }

    public override void EnterState()
    {
        // play ドンドンドン sound
        //             // check voice
        //             // check hand gesture
        // add events to capture them and once done, call another function that will check the current score and the time remaining, if time remaining, transition to idle, if not, go to next scene
    }

    public override void UpdateState() { }

    public override void ExitState() { }

    public override PlayerStateManager.PlayerState GetNextState()
    {
        // throw new System.NotImplementedException();
        return PlayerStateManager.PlayerState.Idle;
    }
}