using System.Collections.Generic;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    [SerializeField] private int _maxRankingSize = 20;

    private PlayerDataManager _playerDataManager;
    private RankingFileManager _rankingFile;
    private List<PlayerData> _playerDatas = new();

    private void Awake()
    {
        _playerDataManager = PlayerDataManager.Instance;

        if (_playerDataManager == null)
        {
            Debug.LogError("RankingManager: PlayerDataManager is null");
            return;
        }

        _rankingFile = new RankingFileManager();
        _playerDatas = _rankingFile.LoadRankingData();

        // add current player's data and sort
        AddCurrentAndSortData();

        // update current player's data in the file
        _rankingFile.SaveRankingData(_playerDatas);
    }

    public List<PlayerData> GetRankingData() => _playerDatas;

    private void AddCurrentAndSortData()
    {
        // if empty just set current player's data
        if (_playerDatas.Count <= 0)
        {
            _playerDatas.Add(new(1, _playerDataManager.PlayerData.Name, _playerDataManager.PlayerData.Score));
            return;
        }

        if (_playerDatas.Count >= _maxRankingSize)
        {
            // check if current player has enough score (is atleast greater than last player's score)
            bool hasEnoughScore = _playerDataManager.Score > _playerDatas[^1].Score;
            if (!hasEnoughScore) return;
        }

        // add current player's data and sort
        _playerDatas.Add(_playerDataManager.PlayerData);
        _playerDatas.Sort((a, b) => b.Score.CompareTo(a.Score));

        // assign new rank numbers
        for (int i = 0; i < _playerDatas.Count; i++)
            _playerDatas[i] = new(i + 1, _playerDatas[i].Name, _playerDatas[i].Score);

        // if there are more than _maxRankingSize players, remove the last one
        if (_playerDatas.Count > _maxRankingSize) _playerDatas.RemoveAt(_maxRankingSize);
    }
}