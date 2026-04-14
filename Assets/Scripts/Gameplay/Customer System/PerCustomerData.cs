using Unity.Cinemachine;
using UnityEngine;

// Data of a single Seat of a table
// Responsible for:
// Holding data for camera set, food points, customer stand position
// Which seat is already occupied
public class PerCustomerData : MonoBehaviour
{
    [SerializeField] private Transform _foodSpawnPoint;
    [SerializeField] private CinemachineStateDrivenCamera _stateCamera;
    [SerializeField] private Transform _customerStandPoint;

    public Transform FoodSpawnPoint => _foodSpawnPoint;
    public CinemachineStateDrivenCamera StateCamera => _stateCamera;
    public Transform CustomerStandPoint => _customerStandPoint;
    public bool IsOccupied;

    // customer allocated on this seat
    public Customer CustomerAllocated { get; set; }
}