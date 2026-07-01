using UnityEngine;

/// <summary>
/// お客の待機タイマーを開始する
/// 時間切れになった場合：
///     - 悪いレビューを付ける（スコア減点）
///     - OutGoingステートへ遷移する
/// </summary>
public class CustomerReady : BaseState<Customer.CustomerState>
{
    private CustomerStateContext _context;
    private bool _transitionByTimesUp = false;
    private float _waitingTimer;
    private const float _badMoodThreshold = 0.5f;   // 悪い気分を設定するための閾値 (待機時間の50%)

    public CustomerReady(CustomerStateContext context, Customer.CustomerState stateKey) : base(stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        _transitionByTimesUp = false;
        _waitingTimer = 0;

        // 初期の気分を設定
        _context.MoodSetter.SetMood(CustomerMood.Happy);
    }

    public override void UpdateState()
    {
        if (_waitingTimer < _context.WaitingTime) _waitingTimer += Time.deltaTime;
        else
        {
            // 使用中の席を空けて、OutGoingステートへ遷移する
            if (CustomersManager.Instance != null)
                CustomersManager.Instance.UnoccupieSeat(_context.ThisCustomer);
            _transitionByTimesUp = true;

            // スコアを減点
            _context.MoodSetter.SetMood(CustomerMood.Angry);    // 時間切れで退店するため、怒りの気分にする
            if (PlayerDataManager.Instance != null)
                PlayerDataManager.Instance.DeduceScore(_context.ScoreToDeductOnTimesUp);
            return;
        }

        // 悲しい気分にする
        if (_context.MoodSetter.CurrentMood != CustomerMood.Sad && _waitingTimer >= _context.WaitingTime * _badMoodThreshold)
            _context.MoodSetter.SetMood(CustomerMood.Sad);
    }

    public override void ExitState() { }

    public override Customer.CustomerState GetNextState()
    {
        // ** InServiceステートへの遷移はCustomerManagerが行う **
        return _transitionByTimesUp ? Customer.CustomerState.OutGoing : Customer.CustomerState.Ready;
    }
}