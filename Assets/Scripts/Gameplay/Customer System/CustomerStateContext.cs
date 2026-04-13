using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class CustomerStateContext
{
    private NavMeshAgent _customerAgent;
    private float _waitingTime;
    private Customer _thisCustomer;
    private Transform _customerDestroyPoint;

    public NavMeshAgent CustomerAgent => _customerAgent;
    public float WaitingTime => _waitingTime;
    public Customer ThisCustomer => _thisCustomer;
    public Transform CustomerDestroyPoint => _customerDestroyPoint;

    public Transform FoodPoint { get; set; }
    public CinemachineStateDrivenCamera StateCamera { get; set; }
    public Transform CustomerStandPoint { get; set; }

    public CustomerStateContext(Customer thisCustomer, NavMeshAgent customerAgent, float waitingTime, Transform customerDestroyPoint)
    {
        _thisCustomer = thisCustomer;
        _customerAgent = customerAgent;
        _waitingTime = waitingTime;
        _customerDestroyPoint = customerDestroyPoint;
    }
}