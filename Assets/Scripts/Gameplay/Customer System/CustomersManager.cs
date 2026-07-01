using System.Collections.Generic;
using UnityEngine;

// すべてのお客を管理するクラス
// 役割：
// - お客を生成する
// - 退店するお客の席を空席にする
// - 次に接客するお客を選択する
// - アクティブなお客を管理する
// - 現在接客中のお客を管理する
public class CustomersManager : Singleton<CustomersManager>
{
    [SerializeField] private CustomerPool _customerPool;
    [SerializeField] private GameObject _customerPrefab;
    [SerializeField] private Transform _customerSpawnPoint; // レストランの外にあるお客を生成する場所
    [SerializeField] private int _maxCustomerCount = 3; // 同時に店内で席を使用できるお客の最大人数（InComing・Ready・InService）。OutGoingのお客は含まない
    [SerializeField] private float _customerSpawnInterval = 20f;
    [SerializeField] private AnimationCurve _customerSpawnCurve;
    [SerializeField] private float _retryCustomerSelectionAfter = 5f;

    private Customer _currentInServiceCustomer; // 現在接客中のお客
    private PerCustomerData[] _customerSeatDatas;   // レストラン内の全席データの配列
    private List<Customer> _currentActiveCustomers = new(); // InComingまたはReadyステートのお客
    private int _currentNoOfCustomers = 0;
    private bool _waitingToSpawnNextCustomer = true;
    private Timer _timer;

    public Customer CurrentCustomer => _currentInServiceCustomer;

    void OnEnable()
    {
        PlayerDoingMoeMoe.OnMoeMoeCompleted += SelectNextCustomer;
    }

    void OnDisable()
    {
        PlayerDoingMoeMoe.OnMoeMoeCompleted -= SelectNextCustomer;
    }

    private void Start()
    {
        // initialize data
        InitializeData();

        // タイマーを取得する
        _timer = FindAnyObjectByType<Timer>();
        // すべての席データを取得する
        _customerSeatDatas = FindObjectsByType<PerCustomerData>(FindObjectsSortMode.None);

        // CustomerPoolを設定する
        _customerPool.CustomerPrefab = _customerPrefab;
        _customerPool.CustomerSpawnPoint = _customerSpawnPoint;

        // 最初のお客を生成する 
        SpawnCustomer();
        SelectNextCustomer();
    }

    private void Update()
    {
        if (!_waitingToSpawnNextCustomer && _currentNoOfCustomers < _maxCustomerCount)
        {
            // 次のお客を生成する
            SpawnCustomer();
        }
    }

    // ** GlobalDataからデータを初期化する **
    private void InitializeData()
    {
        if (GlobalData.Instance == null) return;

        CustomerManagerSettings cms = GlobalData.Instance.CustomerManagerData;

        _maxCustomerCount = cms.maxCustomerCount;
        _customerSpawnInterval = cms.customerSpawnInterval;
        _customerSpawnCurve = cms.customerSpawnCurve;
        _retryCustomerSelectionAfter = cms.retryCustomerSelectionAfter;
    }

    private PerCustomerData GetRandomCustomerData()
    {
        return _customerSeatDatas.Length > 0 ? _customerSeatDatas[Random.Range(0, _customerSeatDatas.Length)] : null;
    }

    private void SpawnCustomer()
    {
        int maxTries = _customerSeatDatas.Length;
        PerCustomerData currentCustomerData = GetRandomCustomerData();

        // 席が空いていることを確認する
        while (maxTries >= 0 && currentCustomerData != null && currentCustomerData.IsOccupied)
        {
            maxTries--;
            currentCustomerData = GetRandomCustomerData();
        }

        // 空いている席が見つからない場合
        if (maxTries < 0 || currentCustomerData == null || currentCustomerData.IsOccupied)
        {
            // 再び待機時間へ切り替える
            _waitingToSpawnNextCustomer = true;
            StartCoroutine(WaitTimer.WaitFor(GetNextCustomerSelectionTime(), () => _waitingToSpawnNextCustomer = false));
            return;
        }

        // レストランの外にお客を生成する
        Customer customer = _customerPool.CurPool.Get();
        // アクティブなお客リストに追加する
        _currentActiveCustomers.Add(customer);
        // お客と席の対応を管理するために追加する
        currentCustomerData.CustomerAllocated = customer;
        currentCustomerData.IsOccupied = true;

        // currentCustomerDataをお客に設定する
        customer.InitializeCustomer(
            currentCustomerData.FoodSpawnPoint,
            currentCustomerData.StateCamera,
            currentCustomerData.CustomerStandPoint
        );

        // お客を席へ移動させる
        customer.TransitionToState(Customer.CustomerState.InComing);

        // 管理対象に追加し、次のお客を生成するまで待機する
        _currentNoOfCustomers++;
        _waitingToSpawnNextCustomer = true;
        StartCoroutine(WaitTimer.WaitFor(GetNextCustomerSelectionTime(), () => _waitingToSpawnNextCustomer = false));
    }

    public void SelectNextCustomer()
    {
        // 現在接客中のお客がいる場合は、先にOutGoingステートへ遷移させる
        if (_currentInServiceCustomer != null)
        {
            // 現在接客中のお客をOutGoingステートへ変更する
            _currentInServiceCustomer.TransitionToState(Customer.CustomerState.OutGoing);
            // この席を空席にする
            UnoccupieSeat(_currentInServiceCustomer);
            // 現在接客中のお客をリセットする
            _currentInServiceCustomer = null;
        }

        // Readyステートのお客が見つからない場合
        if (_currentActiveCustomers.Count == 0 || !_currentActiveCustomers[0].CurrentState.StateKey.Equals(Customer.CustomerState.Ready))
        {
            // しばらく待ってから再試行する
            StartCoroutine(WaitTimer.WaitFor(_retryCustomerSelectionAfter, () => SelectNextCustomer()));
            return;
        }

        // アクティブなお客リストから次のお客を選択する
        Customer nextCustomer = _currentActiveCustomers[0];

        // 接客中になるため、次のお客をアクティブなお客リストから削除する
        _currentActiveCustomers.Remove(nextCustomer);

        // 次のお客を現在接客中のお客として設定する
        _currentInServiceCustomer = nextCustomer;
        // InServiceステートへ変更する
        _currentInServiceCustomer.TransitionToState(Customer.CustomerState.InService);
    }

    // 席からお客を外し、次に使えるよう空席にする
    public void UnoccupieSeat(Customer customerHavingThisSeat)
    {
        // 現在店内にいるお客の人数を減らす
        _currentNoOfCustomers--;

        // 待機時間切れでOutGoingステートへ遷移した場合に備えて、アクティブなお客リストから削除する
        if (_currentActiveCustomers.Contains(customerHavingThisSeat))
            _currentActiveCustomers.Remove(customerHavingThisSeat);

        // データをリセットする
        foreach (PerCustomerData data in _customerSeatDatas)
        {
            if (data.CustomerAllocated == null || !data.CustomerAllocated.Equals(customerHavingThisSeat)) continue;

            data.CustomerAllocated = null;
            data.IsOccupied = false;
            return;
        }
    }

    private float GetNextCustomerSelectionTime()
    {
        float clampedTime = (_timer.TimeLimit - _timer.CurrentTime) / _timer.TimeLimit;
        return _customerSpawnCurve.Evaluate(clampedTime) * _customerSpawnInterval;
    }
}