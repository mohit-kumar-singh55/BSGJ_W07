using UnityEngine;

/// <summary>
/// このステートではカメラを有効にする
/// 料理を生成する
/// </summary>
public class CustomerInService : BaseState<Customer.CustomerState>
{
    private bool _canTransition = false;
    private FoodManager _foodManager;
    private CustomerStateContext _context;

    public static event System.Action OnCustomerEnterInService = delegate { };

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

        // 接客開始時は気分を表示しない
        _context.MoodSetter.SetMood(CustomerMood.None);

        OnCustomerEnterInService?.Invoke();
    }

    public override void UpdateState() { }

    public override void ExitState()
    {
        // turn off camera
        TurnCamera(false);

        // 現在の料理を削除
        _foodManager.DestroyFood();
    }

    public override Customer.CustomerState GetNextState()
    {
        // ** Transition to out-going state is done by customers manager **
        return _canTransition ? Customer.CustomerState.OutGoing : Customer.CustomerState.InService;
    }

    /// <summary>
    /// カメラの有効化/無効化
    /// </summary>
    /// <param name="on"></param>
    public void TurnCamera(bool on = true)
    {
        // _stateCamera.enabled = on;
        _context.StateCamera.gameObject.SetActive(on);
    }
}