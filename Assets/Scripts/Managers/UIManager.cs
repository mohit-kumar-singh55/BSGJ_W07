using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIの管理を行うマネージャークラス
/// </summary>
public class UIManager : Singleton<UIManager>
{
    [Header("Clock UI Settings")]
    [SerializeField] private GameObject _needleUI;
    [SerializeField] private float _rotationOffset = -211f;     // 針UIの現在の回転角
    [SerializeField] private Image _clockRemainingMask;
    [SerializeField] private Color _clockRemainingMaskNormalColor;
    [SerializeField] private Color _clockRemainingMaskCriticalColor;

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

    [Space(10)]
    [Header("Score Popup UI Settings")]
    [SerializeField] private Transform _scorePopupParent;
    [SerializeField] private GameObject _scorePopupPrefab;
    [SerializeField] private float _destroyScorePopupDelay = 1f;

    [Space(10)]
    [Header("Subtitle Moe Kyun UI Settings")]
    [SerializeField] private GameObject _subtitleMoeKyunParent;
    [SerializeField] private Image[] _subtitleMoeKyunImages = new Image[3]; // 0, 1, 2 = moe1, moe2, kyun
    [SerializeField] private Color _subtitleHighlightColor = Color.green;
    [SerializeField] private float _subtitleMoeKyunHideDelay = 1f;

    private void OnEnable()
    {
        PlayerDataManager.OnPlayerScoreChanged += UpdateScoreUpto;
        PlayerPainting.OnPlayerEnterPainting += ShowHideRemainingStokesText;
        PlayerPainting.OnPlayerExitPainting += ShowHideRemainingStokesText;
        SpeechDetector.OnFoundPhraseOccurrence += ChangeSubtitleColorOnOccurrence;
        PlayerDoingMoeMoe.OnMoeMoeStarted += ShowSubtitleMoeKyun;
        PlayerDoingMoeMoe.OnMoeMoeCompleted += HideSubtitleMoeKyun;
    }

    private void OnDisable()
    {
        PlayerDataManager.OnPlayerScoreChanged -= UpdateScoreUpto;
        PlayerPainting.OnPlayerEnterPainting -= ShowHideRemainingStokesText;
        PlayerPainting.OnPlayerExitPainting -= ShowHideRemainingStokesText;
        SpeechDetector.OnFoundPhraseOccurrence -= ChangeSubtitleColorOnOccurrence;
        PlayerDoingMoeMoe.OnMoeMoeStarted -= ShowSubtitleMoeKyun;
        PlayerDoingMoeMoe.OnMoeMoeCompleted -= HideSubtitleMoeKyun;
    }

    // 時計UIの針を回転させる
    // time: [0, 1]
    public void RotateNeedle(float time)
    {
        float t = Mathf.Clamp01(time);
        float angle = t * 360f + _rotationOffset;
        _needleUI.transform.localEulerAngles = new Vector3(0, 0, angle);    // 針UIを回転させる
        _clockRemainingMask.fillAmount = t;   // 針と一緒に残り時間マスクを回転させる
        _clockRemainingMask.color = t <= 0.25f ? _clockRemainingMaskCriticalColor : _clockRemainingMaskNormalColor;   // 残り時間が少なくなったら色を変更する 
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

        // スコアポップアップを生成する
        int scoreDifference = newScore - currentScore;
        if (scoreDifference != 0) SpawnScorePopup(Mathf.Abs(scoreDifference), scoreDifference < 0);
    }

    private void SpawnScorePopup(int score, bool isDeduction = false)
    {
        GameObject popup = Instantiate(_scorePopupPrefab, _scorePopupParent);
        if (popup.TryGetComponent(out TMP_Text popupText))
            popupText.text = (isDeduction ? "-" : "+") + score.ToString();

        // 一定時間後にポップアップを削除する
        Destroy(popup, _destroyScorePopupDelay);
    }

    public void SpawnFeverModeTextPopup(GameObject feverPrefab, Transform parent, float destroyDelay = 1f)
    {
        GameObject popup = Instantiate(feverPrefab, parent);

        // 一定時間後にポップアップを削除する
        Destroy(popup, destroyDelay);
    }

    private void ShowHideRemainingStokesText()
    {
        _remainingStokesText.gameObject.SetActive(!_remainingStokesText.gameObject.activeSelf);
    }

    public void UpdateRemainingStokes(int remainingStokes)
    {
        _remainingStokesText.text = "< " + remainingStokes.ToString() + " >";
    }

    // feverGaugeValue: [0, 1]
    public void UpdateFeverGauge(float feverGaugeValue)
    {
        StartCoroutine(LerpFeverGauge(_feverGaugeInnerImage.fillAmount, feverGaugeValue, _feverGaugeValueLerpDuration));
    }

    public void ShowSubtitleMoeKyun()
    {
        _subtitleMoeKyunParent.SetActive(true);
    }

    public void HideSubtitleMoeKyun()
    {
        // 一定時間後に字幕UIを非表示にする
        StartCoroutine(WaitTimer.WaitFor(_subtitleMoeKyunHideDelay, () =>
        {
            ResetSubtitleColors();  // 非表示時に色をリセットする
            _subtitleMoeKyunParent.SetActive(false);
        }));
    }

    private void ChangeSubtitleColorOnOccurrence(Dictionary<PhraseType, int> occurrencedPhrase)
    {
        if (!_subtitleMoeKyunParent.activeSelf || _subtitleMoeKyunImages.Length < 3) return;

        // moe1, moe2
        if (occurrencedPhrase.TryGetValue(PhraseType.MOE, out int moeCount))
        {
            _subtitleMoeKyunImages[0].color = moeCount > 0 ? _subtitleHighlightColor : Color.white;
            _subtitleMoeKyunImages[1].color = moeCount > 1 ? _subtitleHighlightColor : Color.white;
        }

        // kyun
        if (occurrencedPhrase.TryGetValue(PhraseType.KYUN, out int kyunCount))
        {
            _subtitleMoeKyunImages[2].color = kyunCount > 0 ? _subtitleHighlightColor : Color.white;
        }
    }

    private void ResetSubtitleColors()
    {
        foreach (var image in _subtitleMoeKyunImages)
            image.color = Color.white;
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