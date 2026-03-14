using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ScoreSceneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIDocument _scoreUIPanel;
    [SerializeField] private float _countTimeLimit = 3.0f;

    [Header("Element Names")]
    [SerializeField] private string _scoreName = "ScoreData";

    private Label _scoreText;
    private int _score;

    void Start()
    {
        VisualElement root = _scoreUIPanel.rootVisualElement;

        _scoreText = root.Q<Label>(_scoreName);

        // get current player's score
        _score = PlayerDataManager.Instance.Score;

        // show score
        StartCoroutine(CountUpToScore());

        // TODO: go to next scene if pressed any button
    }

    private IEnumerator CountUpToScore()
    {
        float t = 0;
        int score = 0;

        while (score < _score)
        {
            t += Time.deltaTime / _countTimeLimit;

            score = Mathf.RoundToInt(t * _score);
            _scoreText.text = score.ToString();
            yield return null;
        }

        // show final score to be sure
        _scoreText.text = _score.ToString();
    }
}