using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CustomerDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Customer customer))
        {
            // destroy if in out-going state
            if (customer.CurrentState.StateKey.Equals(Customer.CustomerState.OutGoing))
                Destroy(customer.gameObject);
        }
    }
}