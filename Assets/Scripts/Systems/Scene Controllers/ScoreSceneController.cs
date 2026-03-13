using UnityEngine;
using UnityEngine.UIElements;

public class ScoreSceneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIDocument _scoreUIPanel;

    [Header("Element Names")]
    [SerializeField] private string _scoreName = "ScoreData";

    void Start()
    {
        VisualElement root = _scoreUIPanel.rootVisualElement;

        Label scoreText = root.Q<Label>(_scoreName);

        scoreText.text = PlayerPrefs.GetInt(PLAYER_PREFS.TOTAL_SCORE, 0).ToString();
    }
}