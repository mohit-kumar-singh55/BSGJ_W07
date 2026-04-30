using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private ScreenFader _screenFader;

    private void OnEnable()
    {
        Timer.OnTimesUp += OnTimesUp;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        Timer.OnTimesUp -= OnTimesUp;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    // ! TEMP
    // void Start()
    // {
    //     Time.timeScale = 2f;
    // }

    private void Start()
    {
        // fade in the screen when the scene is loaded
        // if (LoadSceneFader()) _screenFader.FadeInScreen();
        LoadSceneFader();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // fade in the screen when the scene is loaded
        // if (LoadSceneFader()) _screenFader.FadeInScreen();
        LoadSceneFader();
    }

    private bool LoadSceneFader()
    {
        _screenFader = FindAnyObjectByType<ScreenFader>();
        if (_screenFader == null)
        {
            Debug.LogError("GameManager: ScreenFader not found in the scene!");
            return false;
        }
        return true;
    }

    // ** Input System Callbacks
    private void OnGoNextScene(InputValue val) => GoNextScene();

    private void OnGameOver()
    {
        _screenFader.FadeOutScreen(1f, () => SceneManager.LoadScene(SCENES.SCORE));
    }

    public void GoNextScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex + 1;
        index = index == SCENES.DOES_NOT_EXIST ? SCENES.TITLE : index;
        _screenFader.FadeOutScreen(1f, () => SceneManager.LoadScene(index));
    }

    private void OnTimesUp() => GoNextScene();
}