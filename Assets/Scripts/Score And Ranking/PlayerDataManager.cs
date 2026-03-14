using UnityEngine.SceneManagement;

// singleton which holds and manages player data for a single player
// once the player finishes main scene, this player data will be saved in a csv file and then read in the ranking scene
public class PlayerDataManager : Singleton<PlayerDataManager>
{
    private PlayerData _playerData;

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

    public int GetPlayerScore() => _playerData.Score;

    public void SetPlayerName(string name) => _playerData.Name = name;

    public void AddPlayerScore(int score) => _playerData.Score += score;

    public void SetPlayerData(PlayerData playerData)
    {
        _playerData = playerData;
    }

    public void ResetPlayerData() => SetPlayerData(new(-1, "Unknown Player", 0));
}