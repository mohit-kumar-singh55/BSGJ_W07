using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    private int _totalScore = 0;

    public int TotalScore { get => _totalScore; }

    void OnEnable()
    {
        Timer.OnTimesUp += OnTimesUp;
    }

    void OnDisable()
    {
        Timer.OnTimesUp -= OnTimesUp;
    }

    public void AddScore(int score) => _totalScore += score;

    private void OnTimesUp()
    {
        // save the current score
        PlayerPrefs.SetInt(PLAYER_PREFS.TOTAL_SCORE, _totalScore);
        PlayerPrefs.Save();

        // TODO: try to save the playername & score in a text file as csv and then read the file in the ranking scene

        // go to next scene
        GameManager.Instance.GoNextScene();
    }
}