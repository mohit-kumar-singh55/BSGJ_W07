using UnityEngine;

public class FoodManager : Singleton<FoodManager>
{
    [SerializeField] private GameObject[] _foodPrefabs;

    private GameObject _lastSpawnedFood;

    public void SpawnFood()
    {
        _lastSpawnedFood = Instantiate(GetRandomFood(), transform.position, Quaternion.identity);
    }

    public void DestroyFood()
    {
        if (_lastSpawnedFood == null) return;

        Destroy(_lastSpawnedFood);
        _lastSpawnedFood = null;
    }

    private GameObject GetRandomFood() => _foodPrefabs[Random.Range(0, _foodPrefabs.Length)];
}