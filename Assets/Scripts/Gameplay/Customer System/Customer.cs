using Unity.Cinemachine;
using UnityEngine;

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
public class Customer : StateManager<Customer.CustomerState>
{
    public enum CustomerState { InComing, Ready, InService, OutGoing }

    [Tooltip("The point where the food will be spawned")]
    [SerializeField] private Transform _foodSpawnPoint;

    private CinemachineStateDrivenCamera _stateCamera;

    public Transform FoodPoint => _foodSpawnPoint;

    private CustomerStateContext _context;

    private void Awake()
    {
        _stateCamera = GetComponentInChildren<CinemachineStateDrivenCamera>();

        if (_stateCamera == null)
        {
            Debug.LogError("No state camera found in customer!");
            enabled = false;
            return;
        }

        // camera is off by default
        // _stateCamera.enabled = false;
        TurnCamera(false);

        // _stateCamera.Instructions = new CinemachineStateDrivenCamera.Instruction[]
        // {
        //     new() {
        //         Camera = "Idle",
        //         m_Weight = 1f
        //     }
        // };

        _context = new CustomerStateContext();
        InitializeStates();
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

    /// <summary>
    /// Turns on or off the camera
    /// </summary>
    /// <param name="on"></param>
    public void TurnCamera(bool on = true)
    {
        // _stateCamera.enabled = on;
        _stateCamera.gameObject.SetActive(on);
    }
}