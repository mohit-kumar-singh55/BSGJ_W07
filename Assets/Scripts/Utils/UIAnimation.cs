using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation
{
    public static IEnumerator ZoomIn(float time, Graphic graphic, Image image, Action onComplete = null)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / time;
            // image.rectTransform.localScale = Vector3.one * t; // simple linear zoom
            graphic.rectTransform.localScale = Vector3.one * t; // simple linear zoom
            yield return null;
        }

        onComplete?.Invoke();
    }

    public static IEnumerator ZoomIn(float time, Graphic graphic, AnimationCurve curve, Action onComplete = null)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / time;
            graphic.rectTransform.localScale = Vector3.one * curve.Evaluate(t);   // curve zoom
            yield return null;
        }

        onComplete?.Invoke();
    }

    public static IEnumerator ZoomOut(float time, Graphic graphic, Action onComplete = null)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / time;
            graphic.rectTransform.localScale = Vector3.one * (1f - t); // simple linear zoom
            yield return null;
        }

        onComplete?.Invoke();
    }

    public static IEnumerator ZoomOut(float time, Graphic graphic, AnimationCurve curve, Action onComplete = null)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / time;
            graphic.rectTransform.localScale = Vector3.one * curve.Evaluate(t);   // curve zoom
            yield return null;
        }

        onComplete?.Invoke();
    }

    public static IEnumerator FadeIn(float time, float endAlpha, Image image, Action onComplete = null)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / time;

            Color color = image.color;
            color.a = Mathf.Lerp(0f, endAlpha, t); // simple linear fade in
            image.color = color;
            yield return null;
        }

        onComplete?.Invoke();
    }

    public static IEnumerator FadeOut(float time, float startAlpha, Image image, Action onComplete = null)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / time;
            Color color = image.color;
            color.a = Mathf.Lerp(startAlpha, 0f, t);
            image.color = color;
            yield return null;
        }

        onComplete?.Invoke();
    }
}