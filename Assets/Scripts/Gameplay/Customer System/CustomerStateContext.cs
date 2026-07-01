using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// お客のステートマシン用コンテキスト
/// お客やNavMeshAgentなどの参照を保持する
/// 待機時間、削除地点、気分設定も管理する
/// 待機時間切れで退店した際のスコア減点値も保持する
/// </summary>
public class CustomerStateContext
{
    private NavMeshAgent _customerAgent;
    private float _waitingTime;
    private Customer _thisCustomer;
    private Transform _customerDestroyPoint;
    private MoodSetter _moodSetter;
    private int _scoreToDeductOnTimesUp;   // 待機時間切れで退店した際のスコア減点値

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