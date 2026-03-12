using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerStateManager : StateManager<PlayerStateManager.PlayerState>
{
    public enum PlayerState { Idle, Painting, DoingMoeMoe }

    private PlayerStateContext _context;

    void Awake()
    {
        _context = new PlayerStateContext(GetComponent<Animator>());
    }

    void Start()
    {
        InitializeStates();
    }

    private void InitializeStates()
    {
        // Add states to inherited StateManager "States" dictionary and set Initial State
        States.Add(PlayerState.Idle, new PlayerIdle(_context, PlayerState.Idle));
        States.Add(PlayerState.Painting, new PlayerPainting(_context, PlayerState.Painting));
        States.Add(PlayerState.DoingMoeMoe, new PlayerDoingMoeMoe(_context, PlayerState.DoingMoeMoe));

        // always start in idle
        CurrentState = States[PlayerState.Idle];
    }
}