using Unity.Cinemachine;
using UnityEngine;

// テーブルの1席分のデータ
// 役割：
// カメラセット、料理配置位置、お客の待機位置を保持する
// 席が使用中かどうかを管理する
public class PerCustomerData : MonoBehaviour
{
    [SerializeField] private Transform _foodSpawnPoint;
    [SerializeField] private CinemachineStateDrivenCamera _stateCamera;
    [SerializeField] private Transform _customerStandPoint;

    public Transform FoodSpawnPoint => _foodSpawnPoint;
    public CinemachineStateDrivenCamera StateCamera => _stateCamera;
    public Transform CustomerStandPoint => _customerStandPoint;
    public bool IsOccupied;

    // この席に割り当てられているお客
    public Customer CustomerAllocated { get; set; }
}