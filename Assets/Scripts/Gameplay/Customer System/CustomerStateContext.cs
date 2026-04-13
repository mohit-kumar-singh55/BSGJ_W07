using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class CustomerStateContext
{
    private NavMeshAgent _customerAgent;
    private float _waitingTime;
    private Customer _thisCustomer;
    private Transform _customerDestroyPoint;
    private MoodSetter _moodSetter;
    private int _scoreToDeductOnTimesUp;   // score deduction value when customer leaves due to waiting times up

    public NavMeshAgent CustomerAgent => _customerAgent;
    public float WaitingTime => _waitingTime;
    public Customer ThisCustomer => _thisCustomer;
    public Transform CustomerDestroyPoint => _customerDestroyPoint;
    public MoodSetter MoodSetter => _moodSetter;
    public int ScoreToDeductOnTimesUp => _scoreToDeductOnTimesUp;

    public Transform FoodPoint { get; set; }
    public CinemachineStateDrivenCamera StateCamera { get; set; }
    public Transform CustomerStandPoint { get; set; }

    public CustomerStateContext(Customer thisCustomer, NavMeshAgent customerAgent, float waitingTime, Transform customerDestroyPoint, MoodSetter moodSetter, int scoreToDeductOnTimesUp)
    {
        _thisCustomer = thisCustomer;
        _customerAgent = customerAgent;
        _waitingTime = waitingTime;
        _customerDestroyPoint = customerDestroyPoint;
        _moodSetter = moodSetter;
        _scoreToDeductOnTimesUp = scoreToDeductOnTimesUp;
    }
}