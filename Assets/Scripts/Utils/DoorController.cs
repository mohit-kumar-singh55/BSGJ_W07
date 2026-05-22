using UnityEngine;
using UnityEngine.Events;

public class DoorController : MonoBehaviour
{
    [SerializeField] private UnityEvent _onCustomerPassThroughDoor;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(TAGS.CUSTOEMR)) return;
        _onCustomerPassThroughDoor.Invoke();
    }
}