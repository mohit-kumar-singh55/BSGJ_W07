using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

// Represents a customer
// responsible for:
// giving feedback reaction after getting served
// managing the waiting timer of the customer (not sure)
// if time is up, the customer leaves (state to idle) (giving a bad reaction and decrease in score)

// create state machine with states:
// in-coming -> going inside the restaurant, to its seat, then set state to ready
// ready -> run a timer for customer waiting, timeout -> set state to idle (bad reaction and decrease in score)
// in-service -> currently getting served by player, give feedback reaction and set state to idle
// out-going -> customer is leaving the restaurant, destroy customer (or reuse as pooling) after animation is done
[RequireComponent(typeof(NavMeshAgent))]
public class Customer : StateManager<Customer.CustomerState>
{
    public enum CustomerState { InComing, Ready, InService, OutGoing }

    [Tooltip("Customer's Waiting Time")]
    [SerializeField] private float _waitingTime = 20f;
    [Tooltip("Score deduction value when customer leaves due to waiting times up")]
    [SerializeField] private int _scoreToDeductOnTimesUp = 50;

    private NavMeshAgent _customerAgent;
    private CustomerStateContext _context;
    private Transform _customerDestroyPoint;
    private MoodSetter _moodSetter;

    private void Awake()
    {
        _customerAgent = GetComponent<NavMeshAgent>();
        _moodSetter = GetComponentInChildren<MoodSetter>();
        // Point where customer will go back when in out-going state to get destroyed
        _customerDestroyPoint = FindAnyObjectByType<CustomerDestroyer>().transform;

        if (_moodSetter == null)
            Debug.LogError("MoodSetter component not found in children of Customer!");

        _context = new CustomerStateContext(this, _customerAgent, _waitingTime, _customerDestroyPoint, _moodSetter, _scoreToDeductOnTimesUp);
        InitializeStates();
    }

    public void InitializeCustomer(Transform foodSpawnPoint, CinemachineStateDrivenCamera stateCamera, Transform customerStandPoint)
    {
        _context.FoodPoint = foodSpawnPoint;
        _context.StateCamera = stateCamera;
        _context.CustomerStandPoint = customerStandPoint;
    }

    private void InitializeStates()
    {
        // Add states to inherited StateManager "States" dictionary and set Initial State
        States.Add(CustomerState.InComing, new CustomerInComing(_context, CustomerState.InComing));
        States.Add(CustomerState.Ready, new CustomerReady(_context, CustomerState.Ready));
        States.Add(CustomerState.InService, new CustomerInService(_context, CustomerState.InService));
        States.Add(CustomerState.OutGoing, new CustomerOutGoing(_context, CustomerState.OutGoing));

        // always start in idle
        CurrentState = States[CustomerState.InComing];
    }
}