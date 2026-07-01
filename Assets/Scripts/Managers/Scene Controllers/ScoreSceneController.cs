using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ScoreSceneController画面のコントローラークラス
/// </summary>
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

        // 現在のプレイヤーのスコアを取得する
        _score = PlayerDataManager.Instance.Score;

        // スコアを表示する
        StartCoroutine(Counter.CountUpTo(
            0,
            _score,
            _countTimeLimit,
            (score) => _scoreText.text = score.ToString(),
            (score) => _scoreText.text = _score.ToString()
        ));

        // カーソルを表示する
        UnityEngine.Cursor.visible = true;
    }
}