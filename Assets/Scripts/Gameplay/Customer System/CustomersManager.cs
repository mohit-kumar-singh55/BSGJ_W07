using UnityEngine;

// Manages all the customers
// responsible for:
// randomly selecting the next customer
// giving the selected customer detail to camaramanager for camera change

// TODO: seperate customer from (table + chair + camera set + food point)
// TODO: take list of all the customer prefabs & list of all table set
// TODO: select random table set, then spawn random customer and when the customer is in ready state, assign the food point and camera set to that customer
public class CustomersManager : Singleton<CustomersManager>
{
    [SerializeField] private Customer[] customers;
    [SerializeField] private int _maxCustomerCount = 3;

    private Customer _currentCustomer;

    public Customer CurrentCustomer => _currentCustomer;

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
        // select random first customer and set camera on
        _currentCustomer = GetRandomCustomer();
        _currentCustomer.TurnCamera(true);
    }

    private Customer GetRandomCustomer() => customers[Random.Range(0, customers.Length)];

    public void SelectNextCustomer()
    {
        // TODO: check if customer is in ready state
        Customer nextCustomer = GetRandomCustomer();
        // make sure next customer is different
        while (nextCustomer == _currentCustomer) nextCustomer = GetRandomCustomer();

        // set current customer's camera off
        _currentCustomer.TurnCamera(false);

        // set next customer's camera on
        _currentCustomer = nextCustomer;
        _currentCustomer.TurnCamera(true);
    }
}
