/// <summary>
/// go to the customer destroy point
/// and it will be destroyed automatically as it enters the trigger collider
/// </summary>
public class CustomerOutGoing : BaseState<Customer.CustomerState>
{
    private CustomerStateContext _context;
    private bool _canTransition = false;

    public CustomerOutGoing(CustomerStateContext context, Customer.CustomerState stateKey) : base(stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        _context.CustomerAgent.SetDestination(_context.CustomerDestroyPoint.position);
    }

    public override void UpdateState() { }

    public override void ExitState() { }

    public override Customer.CustomerState GetNextState()
    {
        // ! this never gonna run as customer will be destroyed as soon as it goes out of the resturant
        return _canTransition ? Customer.CustomerState.InComing : Customer.CustomerState.OutGoing;
    }
}