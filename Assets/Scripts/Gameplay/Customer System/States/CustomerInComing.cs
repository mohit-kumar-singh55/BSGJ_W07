using UnityEngine;

/// <summary>
/// Moves the customer to the seat
/// </summary>
public class CustomerInComing : BaseState<Customer.CustomerState>
{
    private CustomerStateContext _context;
    private bool _canTransition = false;

    public CustomerInComing(CustomerStateContext context, Customer.CustomerState stateKey) : base(stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        _context.CustomerAgent.SetDestination(_context.CustomerStandPoint.position);
    }

    public override void UpdateState() { }

    public override void ExitState() { }

    public override Customer.CustomerState GetNextState()
    {
        _canTransition = Mathf.Approximately(_context.CustomerAgent.remainingDistance, 0);
        return _canTransition ? Customer.CustomerState.Ready : Customer.CustomerState.InComing;
    }
}