/// <summary>
/// お客削除ポイントへ移動する
/// トリガーコライダーに入ると自動で削除される
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
        // ! レストランの外へ出るとすぐに削除されるため、ここは実行されない
        return _canTransition ? Customer.CustomerState.InComing : Customer.CustomerState.OutGoing;
    }
}