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
        StartCoroutine(Counter.CountUpTo(
            0,
            _score,
            _countTimeLimit,
            (score) => _scoreText.text = score.ToString(),
            (score) => _scoreText.text = _score.ToString()
        ));

        // TODO: go to next scene if pressed any button
    }
}