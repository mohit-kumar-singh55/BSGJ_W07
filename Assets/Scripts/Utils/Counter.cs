using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// このクラスは、指定された範囲内でカウントアップする機能を提供します。
/// </summary>
public class Counter
{
    public static IEnumerator CountUpTo(int from, int to, float countTimeLimit, Action<int> onUpdate = null, Action<int> onComplete = null)
    {
        float t = 0;
        int current = from;

        while (current < to)
        {
            t += Time.deltaTime / countTimeLimit;

            current = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
            onUpdate?.Invoke(current);
            yield return null;
        }

        onComplete?.Invoke(current);
    }
}