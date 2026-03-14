using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    void OnEnable()
    {
        Timer.OnTimesUp += OnTimesUp;
    }

    void OnDisable()
    {
        Timer.OnTimesUp -= OnTimesUp;
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
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

    private void OnTimesUp()
    {
        // TODO: try to save the playername & score in a text file as csv and then read the file in the ranking scene

        // go to next scene
        GoNextScene();
    }
}