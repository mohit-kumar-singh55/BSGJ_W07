using UnityEngine;

public class FoodManager : Singleton<FoodManager>
{
    [SerializeField] private float _spawnFoodDelay = 2f;
    [SerializeField] private GameObject[] _foodPrefabs;

    private GameObject _lastSpawnedFood;
    private CustomersManager _customersManager;

    void Start()
    {
        _customersManager = CustomersManager.Instance;

        if (_customersManager == null)
        {
            Debug.LogError("FoodManager: CustomersManager is null");
        }
    }

    public void SpawnFood(Transform foodSpawnPoint)
    {
        if (CustomersManager.Instance.CurrentCustomer == null) return;

        StopAllCoroutines();
        StartCoroutine(WaitTimer.WaitFor(_spawnFoodDelay, () =>
        {
            _lastSpawnedFood = Instantiate(GetRandomFood(), foodSpawnPoint.position, foodSpawnPoint.rotation);
        }));
    }

    public void DestroyFood()
    {
        if (_lastSpawnedFood == null) return;

        Destroy(_lastSpawnedFood);
        _lastSpawnedFood = null;
    }

    private GameObject GetRandomFood() => _foodPrefabs[Random.Range(0, _foodPrefabs.Length)];
}