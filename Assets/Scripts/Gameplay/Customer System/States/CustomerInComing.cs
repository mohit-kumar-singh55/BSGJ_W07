using UnityEngine.AI;

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
        // _canTransition = Mathf.Approximately(_context.CustomerAgent.remainingDistance, 0);
        // _canTransition = _context.CustomerAgent.remainingDistance <= _context.CustomerAgent.stoppingDistance;
        // _canTransition = !_context.CustomerAgent.pathPending;
        NavMeshAgent agent = _context.CustomerAgent;

        bool reached = !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f);

        _canTransition = reached;

        // set final rotation towards the table
        if (_canTransition) _context.ThisCustomer.transform.rotation = _context.CustomerStandPoint.rotation;

        // transition to ready
        return _canTransition ? Customer.CustomerState.Ready : Customer.CustomerState.InComing;
    }
}