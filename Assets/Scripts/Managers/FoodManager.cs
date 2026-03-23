using System.Collections;
using UnityEngine;

public class FoodManager : Singleton<FoodManager>
{
    [SerializeField] private float _spawnFoodDelay = 2f;
    [SerializeField] private GameObject[] _foodPrefabs;

    private GameObject _lastSpawnedFood;
    private CustomersManager _customersManager;

    void OnEnable()
    {
        PlayerIdle.OnPlayerEnterIdle += SpawnFood;
        PlayerDoingMoeMoe.OnMoeMoeCompleted += DestroyFood;
    }

    void OnDisable()
    {
        PlayerIdle.OnPlayerEnterIdle -= SpawnFood;
        PlayerDoingMoeMoe.OnMoeMoeCompleted -= DestroyFood;
    }

    void Start()
    {
        _customersManager = CustomersManager.Instance;

        if (_customersManager == null)
        {
            Debug.LogError("FoodManager: CustomersManager is null");
        }

        // _customersManager.OnCustomerReady += SpawnFood;
    }

    public void SpawnFood()
    {
        if (CustomersManager.Instance.CurrentCustomer == null) return;

        StopAllCoroutines();

        Transform foodSpawnTrans = CustomersManager.Instance.CurrentCustomer.FoodPoint;
        StartCoroutine(SpawnFoodDelayed(GetRandomFood(), foodSpawnTrans, _spawnFoodDelay));
    }

    public void DestroyFood()
    {
        if (_lastSpawnedFood == null) return;

        Destroy(_lastSpawnedFood);
        _lastSpawnedFood = null;
    }

    private GameObject GetRandomFood() => _foodPrefabs[Random.Range(0, _foodPrefabs.Length)];

    private IEnumerator SpawnFoodDelayed(GameObject food, Transform foodTransform, float delay)
    {
        yield return new WaitForSeconds(delay);
        _lastSpawnedFood = Instantiate(GetRandomFood(), foodTransform.position, foodTransform.rotation);
    }
}