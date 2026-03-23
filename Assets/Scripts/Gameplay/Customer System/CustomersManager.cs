using UnityEngine;

// Manages all the customers
// responsible for:
// randomly selecting the next customer
// giving the selected customer detail to camaramanager for camera change
public class CustomersManager : MonoBehaviour
{
    [SerializeField] private Customer[] customers;

    private Customer _currentCustomer;

    public void SelectNextCustomer()
    {
        // TODO: check if customer is in ready state
        Customer nextCustomer = customers[Random.Range(0, customers.Length)];
        // TODO: set current customer's camera off
        _currentCustomer.TurnCamera(false);
        // TODO: set next customer's camera on
        _currentCustomer = nextCustomer;
        _currentCustomer.TurnCamera(true);
    }
}
