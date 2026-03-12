using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        // PlayerHealth.OnPlayerDeath += OnGameOver;
        // GhostHealth.OnPlayerWin += GoNextLevel;
    }

    void OnDisable()
    {
        // PlayerHealth.OnPlayerDeath -= OnGameOver;
        // GhostHealth.OnPlayerWin -= GoNextLevel;
    }

    // ** Input System Callbacks
    private void OnGoNextScene(InputValue val) => GoNextScene();

    void OnGameOver()
    {
        // 今のレベルを再プレイする
        SceneManager.LoadScene(SCENES.SCORE);
    }

    public void GoNextScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex + 1;
        index = index == SCENES.DOES_NOT_EXIST ? SCENES.TITLE : index;
        SceneManager.LoadScene(index);
    }

    // void OnGameClear()
    // {
    //     // ゲームクリアしているのでレベル1からやり直す
    //     PlayerPrefs.SetInt(PLAYER_PREFS.LAST_PLAYING_LEVEL, SCENES.LEVEL_1);
    //     SceneManager.LoadScene(SCENES.GAME_CLEAR);
    // }
}