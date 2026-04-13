using UnityEngine;

/// <summary>
/// turn-on the camera
/// spawn food
/// </summary>
public class CustomerInService : BaseState<Customer.CustomerState>
{
    private bool _canTransition = false;
    private FoodManager _foodManager;
    private CustomerStateContext _context;


    public CustomerInService(CustomerStateContext context, Customer.CustomerState stateKey) : base(stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        _foodManager = FoodManager.Instance;
        if (!_foodManager)
        {
            Debug.LogError("Food Manager not found!");
            return;
        }

        // turn on camera
        TurnCamera(true);

        // spawn food
        _foodManager.SpawnFood(_context.FoodPoint);


        // dont show mood when started servicing
        _context.MoodSetter.SetMood(CustomerMood.None);
    }

    public override void UpdateState() { }

    public override void ExitState()
    {
        // turn off camera
        TurnCamera(false);

        // destroy current food
        _foodManager.DestroyFood();
    }

    public override Customer.CustomerState GetNextState()
    {
        // ** Transition to out-going state is done by customers manager **
        return _canTransition ? Customer.CustomerState.OutGoing : Customer.CustomerState.InService;
    }

    /// <summary>
    /// Turns on or off the camera
    /// </summary>
    /// <param name="on"></param>
    public void TurnCamera(bool on = true)
    {
        // _stateCamera.enabled = on;
        _context.StateCamera.gameObject.SetActive(on);
    }
}