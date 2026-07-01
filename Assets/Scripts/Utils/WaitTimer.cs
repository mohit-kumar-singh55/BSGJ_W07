using System.Collections;
using UnityEngine;

/// <summary>
/// 待機タイマーを管理するクラス
/// </summary>
public class WaitTimer
{
    public static IEnumerator WaitFor(float seconds, System.Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }
}