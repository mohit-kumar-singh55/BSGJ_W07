using UnityEngine;

public class GlobalData : Singleton<GlobalData>
{
    [SerializeField] public GlobalDataSO globalDataSO;

    public TimerSettings TimerData => globalDataSO.timerSettings;
    public FeverModeSettings FeverModeData => globalDataSO.feverModeSettings;
    public CustomerSettings CustomerData => globalDataSO.customerSettings;
    public CustomerManagerSettings CustomerManagerData => globalDataSO.customerManagerSettings;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        if (globalDataSO == null)
        {
            Debug.LogError("GlobalData: GlobalDataSO is null");
            enabled = false;
        }
    }
}