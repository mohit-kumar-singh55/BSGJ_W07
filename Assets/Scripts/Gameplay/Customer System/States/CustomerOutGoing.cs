public class CustomerOutGoing : BaseState<Customer.CustomerState>
{
    private bool _canTransition = false;

    public CustomerOutGoing(CustomerStateContext context, Customer.CustomerState stateKey) : base(stateKey) { }

    public override void EnterState() { }

    public override void UpdateState() { }

    public override void ExitState() { }

    public override Customer.CustomerState GetNextState()
    {
        // ! need to remove this incoming state, because the customer will be destroyed after this state, so it will never transition back to incoming
        return _canTransition ? Customer.CustomerState.InComing : Customer.CustomerState.OutGoing;
    }
}