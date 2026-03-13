using UnityEngine;

public class PlayerIdle : BaseState<PlayerStateManager.PlayerState>
{
    private float _idleTimer = 5f;
    private float _spawnFoodAfter = 2f;
    private bool _foodSpawned = false;
    private bool _canTransition = false;

    public PlayerIdle(PlayerStateContext context, PlayerStateManager.PlayerState stateKey) : base(stateKey) { }

    public override void EnterState()
    {
        // just wait for sometime and then transition to painting
        _idleTimer = 5f;
        _spawnFoodAfter = 2f;
        _foodSpawned = false;
        _canTransition = false;

        // spawn food
        // FoodManager.Instance.SpawnFood();
    }

    public override void UpdateState()
    {
        if (_idleTimer <= 0) _canTransition = true;
        else _idleTimer -= Time.deltaTime;

        // ! temp
        if (!_foodSpawned && _spawnFoodAfter <= 0)
        {
            // spawn food
            _foodSpawned = true;
            FoodManager.Instance.SpawnFood();
        }
        else _spawnFoodAfter -= Time.deltaTime;
    }

    public override void ExitState() { }

    public override PlayerStateManager.PlayerState GetNextState()
    {
        return _canTransition ? PlayerStateManager.PlayerState.Painting : PlayerStateManager.PlayerState.Idle;
    }
}

// create timer
// create overall score manager