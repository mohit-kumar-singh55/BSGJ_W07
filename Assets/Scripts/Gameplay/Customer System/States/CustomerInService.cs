public class CustomerInService : BaseState<Customer.CustomerState>
{
    private bool _canTransition = false;

    public CustomerInService(CustomerStateContext context, Customer.CustomerState stateKey) : base(stateKey) { }

    public override void EnterState() { }

    public override void UpdateState() { }

    public override void ExitState() { }

    public override Customer.CustomerState GetNextState()
    {
        return _canTransition ? Customer.CustomerState.OutGoing : Customer.CustomerState.InService;
    }
}