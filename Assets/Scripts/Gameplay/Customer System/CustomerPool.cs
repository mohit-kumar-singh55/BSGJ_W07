using UnityEngine;

public class CustomerPool : Pool<Customer>
{
    [HideInInspector] public GameObject CustomerPrefab;
    [HideInInspector] public Transform CustomerSpawnPoint;

    private void OnEnable()
    {
        CustomerDestroyer.OnCustomerDestroy += ReturnToPool;
    }

    private void OnDisable()
    {
        CustomerDestroyer.OnCustomerDestroy -= ReturnToPool;
    }

    protected override Customer CreateObject()
    {
        GameObject customerObj = Instantiate(CustomerPrefab, CustomerSpawnPoint.position, Quaternion.identity, CustomerSpawnPoint);
        // TODO: stop from moving and hide
        customerObj.SetActive(false);
        return customerObj.GetComponent<Customer>();
    }

    protected override void OnDestroyFromPool(Customer pooledObject)
    {
        Destroy(pooledObject.gameObject);
    }

    protected override void OnTakeFromPool(Customer pooledObject)
    {
        pooledObject.gameObject.SetActive(true);
    }

    protected override void OnReturnToPool(Customer pooledObject)
    {
        pooledObject.gameObject.SetActive(false);
        pooledObject.transform.position = CustomerSpawnPoint.position;  // reset position
    }

    private void ReturnToPool(MoodSetter moodSetter)
    {
        Customer customer = moodSetter.GetComponentInParent<Customer>();
        if (customer == null) return;
        CurPool.Release(customer);
    }
}