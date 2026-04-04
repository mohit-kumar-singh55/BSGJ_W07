using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [Header("Clock UI Settings")]
    [SerializeField] private GameObject _needleUI;
    [SerializeField] private float _rotationOffset = -211f;     // current rotation of needle ui

    [Space(10)]
    [Header("Score UI Settings")]
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private float _timeToCount = 3f;

    [Space(10)]
    [Header("Remaining Stokes UI Settings")]
    [SerializeField] private TMP_Text _remainingStokesText;

    [Space(10)]
    [Header("Fever Gauge UI Settings")]
    [SerializeField] private Image _feverGaugeInnerImage;
    [SerializeField] private float _feverGaugeValueLerpDuration = 1f;
    // ! temp
    [SerializeField] private TMP_Text _feverGaugeText;

    [Space(10)]
    [Header("Score Popup UI Settings")]
    [SerializeField] private Transform _scorePopupParent;
    [SerializeField] private GameObject _scorePopupPrefab;
    [SerializeField] private float _destroyScorePopupDelay = 1f;

    private void OnEnable()
    {
        PlayerDataManager.OnPlayerScoreChanged += UpdateScoreUpto;
        PlayerPainting.OnPlayerEnterPainting += ShowRemainingStokesText;
        PlayerPainting.OnPlayerExitPainting += ShowRemainingStokesText;
    }

    private void OnDisable()
    {
        PlayerDataManager.OnPlayerScoreChanged -= UpdateScoreUpto;
        PlayerPainting.OnPlayerEnterPainting -= ShowRemainingStokesText;
        PlayerPainting.OnPlayerExitPainting -= ShowRemainingStokesText;
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

        // Spawn score popup
        int scoreDifference = newScore - currentScore;
        if (scoreDifference > 0) SpawnScorePopup(scoreDifference);
    }

    private void SpawnScorePopup(int score)
    {
        GameObject popup = Instantiate(_scorePopupPrefab, _scorePopupParent);
        if (popup.TryGetComponent(out TMP_Text popupText))
            popupText.text = "+" + score.ToString();

        // Destroy the popup after a delay
        Destroy(popup, _destroyScorePopupDelay);
    }

    private void ShowRemainingStokesText()
    {
        _remainingStokesText.gameObject.SetActive(!_remainingStokesText.gameObject.activeSelf);
    }

    public void UpdateRemainingStokes(int remainingStokes)
    {
        _remainingStokesText.text = "< " + remainingStokes.ToString() + " >";
    }

    // ! temp
    public void UpdateFeverGaugeText(bool isFeverMode = true)
    {
        _feverGaugeText.text = "FEVER MODE : " + (isFeverMode ? "ON" : "OFF");
    }

    // feverGaugeValue: [0, 1]
    public void UpdateFeverGauge(float feverGaugeValue)
    {
        StartCoroutine(LerpFeverGauge(_feverGaugeInnerImage.fillAmount, feverGaugeValue, _feverGaugeValueLerpDuration));
    }

    private IEnumerator LerpFeverGauge(float startValue, float endValue, float duration)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            _feverGaugeInnerImage.fillAmount = Mathf.Lerp(startValue, endValue, t);
            yield return null;
        }
    }
}