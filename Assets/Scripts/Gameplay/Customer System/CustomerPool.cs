using UnityEngine;

/// <summary>
/// CustomerPoolはCustomerオブジェクトを再利用するためのオブジェクトプール
/// 頻繁な生成・削除を避けるため、Customerオブジェクトの生成・有効化・無効化・削除を管理する
/// </summary>
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
        // Customerオブジェクトを無効化してプールに戻す
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
        pooledObject.transform.position = CustomerSpawnPoint.position;  // 位置をリセットする
    }

    private void ReturnToPool(MoodSetter moodSetter)
    {
        Customer customer = moodSetter.GetComponentInParent<Customer>();
        if (customer == null) return;
        CurPool.Release(customer);
    }
}