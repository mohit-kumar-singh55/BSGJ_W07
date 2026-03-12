using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RankDataBinder : MonoBehaviour
{
    [SerializeField] private UIDocument _rankUI;
    [SerializeField] private string _playerDataListName = "RankingList";

    private VisualElement _root;
    private ListView _playerDataList;
    private List<PlayerData> _playerDatas = new();

    void Start()
    {
        _root = _rankUI.rootVisualElement;
        _playerDataList = _root.Q<ListView>(_playerDataListName);

        CreateTestData(20);

        // _playerDataList.makeItem = MakeItem;
        _playerDataList.itemsSource = _playerDatas;
        _playerDataList.Rebuild();
    }

    private void CreateTestData(int count = 5)
    {
        for (int i = 0; i < count; i++)
        {
            _playerDatas.Add(new PlayerData(i + 1, "Player " + (i + 1), i + 3));
        }
    }
}