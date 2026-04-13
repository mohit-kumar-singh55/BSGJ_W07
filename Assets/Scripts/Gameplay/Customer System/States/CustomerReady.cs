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
    private const float _badMoodThreshold = 0.5f;   // threshold for setting bad mood (50% of waiting time)

    public CustomerReady(CustomerStateContext context, Customer.CustomerState stateKey) : base(stateKey)
    {
        _context = context;
    }

    public override void EnterState()
    {
        _transitionByTimesUp = false;
        _waitingTimer = 0;

        // set happy mood
        _context.MoodSetter.SetMood(CustomerMood.Happy);
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
            _context.MoodSetter.SetMood(CustomerMood.Angry);    // angry mood when goting outside due to times up
            if (PlayerDataManager.Instance != null)
                PlayerDataManager.Instance.DeduceScore(_context.ScoreToDeductOnTimesUp);
            return;
        }

        // set sad mood
        if (_context.MoodSetter.CurrentMood != CustomerMood.Sad && _waitingTimer >= _context.WaitingTime * _badMoodThreshold)
            _context.MoodSetter.SetMood(CustomerMood.Sad);
    }

    public override void ExitState() { }

    public override Customer.CustomerState GetNextState()
    {
        // ** Transition to In-service state is done by the customers manager **
        return _transitionByTimesUp ? Customer.CustomerState.OutGoing : Customer.CustomerState.Ready;
    }
}