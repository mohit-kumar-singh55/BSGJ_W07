using UnityEngine.AI;

/// <summary>
/// このステートでは、お客を席まで移動させる
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
        NavMeshAgent agent = _context.CustomerAgent;

        bool reached = !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f);

        _canTransition = reached;

        // テーブルの方向へ最終的な向きを設定
        if (_canTransition) _context.ThisCustomer.transform.rotation = _context.CustomerStandPoint.rotation;

        // Readyステートへ遷移
        return _canTransition ? Customer.CustomerState.Ready : Customer.CustomerState.InComing;
    }
}