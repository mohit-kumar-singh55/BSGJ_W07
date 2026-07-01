using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シングルプレイ用のプレイヤーデータを保持・管理するシングルトン
// メインシーン終了後、このプレイヤーデータをCSVファイルに保存し、ランキングシーンで読み込む
/// </summary>
public class PlayerDataManager : Singleton<PlayerDataManager>
{
    private PlayerData _playerData;

    public PlayerData PlayerData => _playerData;
    public int Score => _playerData.Score;

    public static event System.Action<int, int> OnPlayerScoreChanged = delegate { };

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // setting default player data
        ResetPlayerData();

        // reset player data whenever title scene is loaded
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.buildIndex == SCENES.TITLE) ResetPlayerData();
        };
    }

    public void SetPlayerName(string name) => _playerData.Name = name;

    public void AddPlayerScore(int scoreToAdd)
    {
        int newScore = _playerData.Score + scoreToAdd;
        OnPlayerScoreChanged?.Invoke(_playerData.Score, newScore); // (current, new)
        _playerData.Score = newScore;
    }

    public void DeduceScore(int scoreToDeduce)
    {
        int newScore = Mathf.Max(0, _playerData.Score - scoreToDeduce); // score cannot be negative
        OnPlayerScoreChanged?.Invoke(_playerData.Score, newScore); // (current, new)
        _playerData.Score = newScore;
    }

    public void SetPlayerData(PlayerData playerData)
    {
        _playerData = playerData;
    }

    public void ResetScore() => _playerData.Score = 0;

    public void ResetPlayerData() => SetPlayerData(new(-1, "Unknown", 0));
}