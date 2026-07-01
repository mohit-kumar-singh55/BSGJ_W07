using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// このクラスは、スクリーンのフェードイン・フェードアウト効果を実装します。
/// </summary>
public class ScreenFader : MonoBehaviour
{
    private GameObject _fader;
    private Image _faderImage;

    private void Awake()
    {
        _fader = transform.GetChild(0).gameObject;
        _faderImage = _fader.GetComponent<Image>();

        if (!_faderImage)
        {
            Debug.LogError("ScreenFader: ScreenFader Image component is missing!");
            enabled = false;
            return;
        }

        // 初期状態はフェードインしている状態
        FadeInScreen(2f);
    }

    public void FadeInScreen(float time = 1f, Action onFadeComplete = null)
    {
        StopAllCoroutines();
        _fader.SetActive(true);
        StartCoroutine(SetColorAlphaValue(true, time, onFadeComplete));
    }

    public void FadeOutScreen(float time = 1f, Action onFadeComplete = null)
    {
        StopAllCoroutines();
        _fader.SetActive(true);
        StartCoroutine(SetColorAlphaValue(false, time, onFadeComplete));
    }

    private void SetAlpha(float alpha)
    {
        Color newColor = _faderImage.color;
        newColor.a = alpha;
        _faderImage.color = newColor;
    }

    /// <summary>
    /// スクリーンをフェードアウト
    /// </summary>
    private IEnumerator SetColorAlphaValue(bool isFadeIn = true, float time = 1f, Action onFadeComplete = null)
    {
        // アルファ値の初期化
        SetAlpha(isFadeIn ? 1f : 0f);

        // アルファ値を変化させる
        float t = 0f;
        while (isFadeIn ? _faderImage.color.a > 0f : _faderImage.color.a < 1f)
        {
            t += Time.deltaTime / time;
            float newAlpha = _faderImage.color.a + (isFadeIn ? -.1f : .1f) * t;
            SetAlpha(Mathf.Clamp01(newAlpha));

            yield return null;
        }

        if (isFadeIn) _fader.SetActive(false);
        onFadeComplete?.Invoke();
    }
}