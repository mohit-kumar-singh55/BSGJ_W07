using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

// お客を表すクラス
// 役割：
// 接客後のリアクションを返す
// お客の待機タイマーを管理する
// 時間切れになった場合は、悪いリアクションをしてスコアを減点し、Idleステートへ戻る

// ステートマシンを作成する：
// InComing -> 店内に入り、席まで移動した後、Readyステートへ遷移
// Ready -> お客の待機タイマーを開始し、時間切れの場合はIdleステートへ遷移（悪いリアクションとスコア減点）
// InService -> プレイヤーに接客されている状態。リアクションを返してIdleステートへ遷移
// OutGoing -> お客が退店する状態。アニメーション終了後に削除する（またはプールで再利用する）
[RequireComponent(typeof(NavMeshAgent))]
public class Customer : StateManager<Customer.CustomerState>
{
    public enum CustomerState { InComing, Ready, InService, OutGoing }

    [Tooltip("お客の待機時間")]
    [SerializeField] private float _waitingTime = 20f;
    [Tooltip("待機時間切れでお客が退店した時のスコア減点値")]
    [SerializeField] private int _scoreToDeductOnTimesUp = 20;

    private NavMeshAgent _customerAgent;
    private CustomerStateContext _context;
    private Transform _customerDestroyPoint;
    private MoodSetter _moodSetter;

    public bool IsInBadMood { get; private set; }

    public static event System.Action<MoodSetter> OnCustomerSpawn = delegate { };

    private void Awake()
    {
        _customerAgent = GetComponent<NavMeshAgent>();
        _moodSetter = GetComponentInChildren<MoodSetter>();
        // OutGoingステートで削除されるために向かう目的地
        _customerDestroyPoint = FindAnyObjectByType<CustomerDestroyer>().transform;

        if (_moodSetter == null)
            Debug.LogError("MoodSetter component not found in children of Customer!");

        // initialize data
        InitializeData();

        _context = new CustomerStateContext(this, _customerAgent, _waitingTime, _customerDestroyPoint, _moodSetter, _scoreToDeductOnTimesUp);
        InitializeStates();

        // updating mood display ui
        OnCustomerSpawn?.Invoke(_moodSetter);
    }

    protected override void Update()
    {
        base.Update();

        // お客が機嫌の悪い状態か確認
        if (_moodSetter.CurrentMood == CustomerMood.Sad)
            IsInBadMood = true;
    }

    public void InitializeCustomer(Transform foodSpawnPoint, CinemachineStateDrivenCamera stateCamera, Transform customerStandPoint)
    {
        _context.FoodPoint = foodSpawnPoint;
        _context.StateCamera = stateCamera;
        _context.CustomerStandPoint = customerStandPoint;
    }

    private void InitializeStates()
    {
        // 継承元のStateManagerのStates辞書にステートを追加し、初期ステートを設定する
        States.Add(CustomerState.InComing, new CustomerInComing(_context, CustomerState.InComing));
        States.Add(CustomerState.Ready, new CustomerReady(_context, CustomerState.Ready));
        States.Add(CustomerState.InService, new CustomerInService(_context, CustomerState.InService));
        States.Add(CustomerState.OutGoing, new CustomerOutGoing(_context, CustomerState.OutGoing));

        // always start in idle
        CurrentState = States[CustomerState.InComing];
    }

    // ** GlobalDataからデータを初期化する **
    private void InitializeData()
    {
        if (GlobalData.Instance == null) return;

        CustomerSettings cs = GlobalData.Instance.CustomerData;

        _waitingTime = cs.waitingTime;
        _scoreToDeductOnTimesUp = cs.scoreToDeductOnTimesUp;
    }
}