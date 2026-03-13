using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreSceneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIDocument _scoreUIPanel;

    [Header("Element Names")]
    [SerializeField] private string _scoreName = "ScoreData";

    private Label _scoreText;
    private int _score;

    void Start()
    {
        VisualElement root = _scoreUIPanel.rootVisualElement;

        _scoreText = root.Q<Label>(_scoreName);

        _score = PlayerPrefs.GetInt(PLAYER_PREFS.TOTAL_SCORE, 0);

        // show score
        StartCoroutine(CountUpToScore());

        // TODO: go to next scene if pressed any button
    }

    private IEnumerator CountUpToScore()
    {
        int score = 0;
        while (score < _score)
        {
            score++;
            _scoreText.text = score.ToString();
            yield return new WaitForSeconds(0.01f);
        }
    }
}