using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ランキングシーンのUIにランキングデータをバインドするクラス
/// </summary>
[RequireComponent(typeof(RankingManager))]
public class RankDataBinder : MonoBehaviour
{
    [SerializeField] private UIDocument _rankUI;
    [SerializeField] private string _playerDataListName = "RankingList";

    private VisualElement _root;
    private ListView _playerDataList;
    private RankingManager _rankingManager;

    void Start()
    {
        _rankingManager = GetComponent<RankingManager>();
        _root = _rankUI.rootVisualElement;
        _playerDataList = _root.Q<ListView>(_playerDataListName);

        // _playerDataList.makeItem = MakeItem;
        _playerDataList.itemsSource = _rankingManager.GetRankingData();
        _playerDataList.Rebuild();
    }
}