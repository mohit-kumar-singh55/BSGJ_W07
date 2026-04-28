using System.Collections.Generic;
using UnityEngine;

// Manages all the customers
// responsible for:
// - spawning customers
// - unoccupying the seats of the customers that are leaving the resturant
// - selecting next customer
// - keeping track of active customers
// - keeping track of current in-service customer
public class CustomersManager : Singleton<CustomersManager>
{
    [SerializeField] private GameObject _customerPrefab;
    [SerializeField] private Transform _customerSpawnPoint; // outside the resturant
    [SerializeField] private int _maxCustomerCount = 3; // max number of customers that can be in the resturant at the same time, that are occupying the seats (in-coming, ready, in-service), not including the customers that are out-going
    [SerializeField] private float _customerSpawnInterval = 20f;
    [SerializeField] private AnimationCurve _customerSpawnCurve;
    [SerializeField] private float _retryCustomerSelectionAfter = 5f;

    private Customer _currentInServiceCustomer; // customer that is being served right now
    private PerCustomerData[] _customerSeatDatas;   // total seats in the resturant
    private List<Customer> _currentActiveCustomers = new(); // customers that are in in-coming/ready state
    private int _currentNoOfCustomers = 0;
    private bool _waitingToSpawnNextCustomer = true;
    private Timer _timer;

    public Customer CurrentCustomer => _currentInServiceCustomer;

    void OnEnable()
    {
        PlayerDoingMoeMoe.OnMoeMoeCompleted += SelectNextCustomer;
    }

    void OnDisable()
    {
        PlayerDoingMoeMoe.OnMoeMoeCompleted -= SelectNextCustomer;
    }

    private void Start()
    {
        // initialize data
        InitializeData();

        // get timer
        _timer = FindAnyObjectByType<Timer>();
        // get all the seats datas
        _customerSeatDatas = FindObjectsByType<PerCustomerData>(FindObjectsSortMode.None);

        // spawn first customer 
        SpawnCustomer();
        SelectNextCustomer();
    }

    private void Update()
    {
        if (!_waitingToSpawnNextCustomer && _currentNoOfCustomers < _maxCustomerCount)
        {
            // spawn next customer
            SpawnCustomer();
        }
    }

    // ** initialize data from GlobalData **
    private void InitializeData()
    {
        if (GlobalData.Instance == null) return;

        CustomerManagerSettings cms = GlobalData.Instance.CustomerManagerData;

        _maxCustomerCount = cms.maxCustomerCount;
        _customerSpawnInterval = cms.customerSpawnInterval;
        _customerSpawnCurve = cms.customerSpawnCurve;
        _retryCustomerSelectionAfter = cms.retryCustomerSelectionAfter;
    }

    private PerCustomerData GetRandomCustomerData()
    {
        return _customerSeatDatas.Length > 0 ? _customerSeatDatas[Random.Range(0, _customerSeatDatas.Length)] : null;
    }

    private void SpawnCustomer()
    {
        int maxTries = _customerSeatDatas.Length;
        PerCustomerData currentCustomerData = GetRandomCustomerData();

        // make sure the seat is unoccupied
        while (maxTries >= 0 && currentCustomerData != null && currentCustomerData.IsOccupied)
        {
            maxTries--;
            currentCustomerData = GetRandomCustomerData();
        }

        // cannot find unoccupied seat
        if (maxTries < 0 || currentCustomerData == null || currentCustomerData.IsOccupied)
        {
            // again to into waiting period
            _waitingToSpawnNextCustomer = true;
            StartCoroutine(WaitTimer.WaitFor(GetNextCustomerSelectionTime(), () => _waitingToSpawnNextCustomer = false));
            return;
        }

        // spawn customer outside the resturant
        GameObject customerObj = Instantiate(_customerPrefab, _customerSpawnPoint.position, Quaternion.identity, _customerSpawnPoint);
        if (customerObj.TryGetComponent(out Customer customer))
        {
            // add into the active customers list
            _currentActiveCustomers.Add(customer);
            // add to keep track of the customer with its seat
            currentCustomerData.CustomerAllocated = customer;
            currentCustomerData.IsOccupied = true;

            // TODO: assign currentCustomerData to customer
            customer.InitializeCustomer(
                currentCustomerData.FoodSpawnPoint,
                currentCustomerData.StateCamera,
                currentCustomerData.CustomerStandPoint
            );

            // keep track & wait to spawn next customer
            _currentNoOfCustomers++;
            _waitingToSpawnNextCustomer = true;
            StartCoroutine(WaitTimer.WaitFor(GetNextCustomerSelectionTime(), () => _waitingToSpawnNextCustomer = false));
        }
        else Destroy(customerObj);
    }

    public void SelectNextCustomer()
    {
        // if there is currently in-service customer, make them out-going first
        if (_currentInServiceCustomer != null)
        {
            // change current customer's in-service state to outgoing
            _currentInServiceCustomer.TransitionToState(Customer.CustomerState.OutGoing);
            // unoccupie this seat
            UnoccupieSeat(_currentInServiceCustomer);
            // reset current in-service customer
            _currentInServiceCustomer = null;
        }

        // cannot find any customer in ready state
        if (_currentActiveCustomers.Count == 0 || !_currentActiveCustomers[0].CurrentState.StateKey.Equals(Customer.CustomerState.Ready))
        {
            // retry after sometime
            StartCoroutine(WaitTimer.WaitFor(_retryCustomerSelectionAfter, () => SelectNextCustomer()));
            return;
        }

        // select next customer from the active customers list
        Customer nextCustomer = _currentActiveCustomers[0];

        // remove next customer from the active customer list as it will be in-service
        _currentActiveCustomers.Remove(nextCustomer);

        // set next customer to current customer
        _currentInServiceCustomer = nextCustomer;
        // change state to in-service
        _currentInServiceCustomer.TransitionToState(Customer.CustomerState.InService);
    }

    // removes the customer from that seat & mark it unoccupied for next use
    public void UnoccupieSeat(Customer customerHavingThisSeat)
    {
        // decrease current number of customers in the restaurant
        _currentNoOfCustomers--;

        // remove from active customers list in case the customer's waiting time is up & it transitioned into out-going state
        if (_currentActiveCustomers.Contains(customerHavingThisSeat))
            _currentActiveCustomers.Remove(customerHavingThisSeat);

        // reset data
        foreach (PerCustomerData data in _customerSeatDatas)
        {
            if (data.CustomerAllocated == null || !data.CustomerAllocated.Equals(customerHavingThisSeat)) continue;

            data.CustomerAllocated = null;
            data.IsOccupied = false;
            return;
        }
    }

    private float GetNextCustomerSelectionTime()
    {
        float clampedTime = (_timer.TimeLimit - _timer.CurrentTime) / _timer.TimeLimit;
        return _customerSpawnCurve.Evaluate(clampedTime) * _customerSpawnInterval;
    }
}