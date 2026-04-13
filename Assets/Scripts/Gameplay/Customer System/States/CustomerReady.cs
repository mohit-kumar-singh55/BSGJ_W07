using UnityEngine;

/// <summary>
/// Starts customer waiting timer
/// if times up:
///     - give bad review (score deduction)
///     - transition to out-going state
/// </summary>
public class CustomerReady : BaseState<Customer.CustomerState>
{
    private CustomerStateContext _context;
    private bool _transitionByTimesUp = false;
    private float _waitingTimer;

    public CustomerReady(CustomerStateContext context, Customer.CustomerState stateKey) : base(stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        _transitionByTimesUp = false;
        _waitingTimer = 0;
    }

    public override void UpdateState()
    {
        if (_waitingTimer < _context.WaitingTime) _waitingTimer += Time.deltaTime;
        else
        {
            // unoccupie the current taken seat & transition to out-going
            if (CustomersManager.Instance != null)
                CustomersManager.Instance.UnoccupieSeat(_context.ThisCustomer);
            _transitionByTimesUp = true;

            // TODO: deduce score
        }
    }

    public override void ExitState() { }

    public override Customer.CustomerState GetNextState()
    {
        // ** Transition to In-service state is done by the customers manager **
        return _transitionByTimesUp ? Customer.CustomerState.OutGoing : Customer.CustomerState.Ready;
    }
}