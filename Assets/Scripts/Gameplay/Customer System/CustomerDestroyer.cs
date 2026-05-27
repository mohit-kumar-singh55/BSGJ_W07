using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CustomerDestroyer : MonoBehaviour
{
    public static event System.Action<MoodSetter> OnCustomerDestroy = delegate { };

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Customer customer))
        {
            // destroy if in out-going state
            if (customer.CurrentState.StateKey.Equals(Customer.CustomerState.OutGoing))
            {
                // remove mood setter from off screen mood displayer ui
                MoodSetter moodSetter = customer.GetComponentInChildren<MoodSetter>();
                if (moodSetter != null) OnCustomerDestroy?.Invoke(moodSetter);
                // Destroy(customer.gameObject);    // instead releasing it from pool
            }
        }
    }
}