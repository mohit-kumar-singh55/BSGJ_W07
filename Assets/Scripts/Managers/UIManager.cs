using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [Header("Clock UI Settings")]
    [SerializeField] private GameObject _needleUI;
    [SerializeField] private float _rotationOffset = -211f;     // current rotation of needle ui

    [Space(10)]
    [Header("Score UI Settings")]
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private float _timeToCount = 3f;

    private void OnEnable()
    {
        PlayerDataManager.OnPlayerScoreChanged += UpdateScoreUpto;
    }

    private void OnDisable()
    {
        PlayerDataManager.OnPlayerScoreChanged -= UpdateScoreUpto;
    }

    // rotate needle of clock ui
    // time: [0, 1]
    public void RotateNeedle(float time)
    {
        float t = Mathf.Clamp01(time);
        float angle = t * -360f + _rotationOffset;
        _needleUI.transform.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void UpdateScoreUpto(int currentScore, int newScore)
    {
        StopAllCoroutines();

        StartCoroutine(Counter.CountUpTo(
            currentScore,
            newScore,
            _timeToCount,
            (score) => _scoreText.text = score.ToString(),
            (score) => _scoreText.text = newScore.ToString()
        ));
    }
}