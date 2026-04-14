using System.Collections;
using UnityEngine;

public class WaitTimer
{
    public static IEnumerator WaitFor(float seconds, System.Action callback)
    {
        yield return new WaitForSeconds(seconds);
        callback?.Invoke();
    }
}