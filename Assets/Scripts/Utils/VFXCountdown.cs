using System;
using System.Collections;
using UnityEngine;

public class VFXCountdown : MonoBehaviour
{
    [Tooltip("Should be in incremental order (0, 1, 2, 3...) and match the countdown seconds")]
    [SerializeField] private GameObject[] _countdownVFXPrefabs;
    [SerializeField] private float _countdownInterval = 2f;

    public float CountdownInterval { set { _countdownInterval = value; } }

    public void StartCountdown(Action onComplete = null)
    {
        StartCoroutine(ShowCountdown(onComplete));
    }

    private IEnumerator ShowCountdown(Action onComplete = null)
    {
        int countdownIndex = _countdownVFXPrefabs.Length - 1;

        while (countdownIndex >= 0)
        {
            Instantiate(_countdownVFXPrefabs[countdownIndex], transform.position, Quaternion.identity);
            yield return new WaitForSeconds(_countdownInterval);

            countdownIndex--;
        }

        onComplete?.Invoke();
    }
}