using UnityEngine;

public class GlobalData : Singleton<GlobalData>
{
    [SerializeField] public GlobalDataSO globalDataSO;

    public FeverModeSettings FeverModeData => globalDataSO.feverModeSettings;
    public CustomerSettings CustomerData => globalDataSO.customerSettings;

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