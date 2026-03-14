using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float _timeLimit = 90f;
    [SerializeField] private TMP_Text _timerText;

    private float currentTime;
    private bool isRunning = true;

    public float CurrentTime => currentTime;

    public static event System.Action OnTimesUp = delegate { };

    private void Start() => currentTime = _timeLimit;

    // ** タイマーの更新 **
    private void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            isRunning = false;

            // stop player state machine
            // go to next scene
            OnTimesUp?.Invoke();
            return;
        }

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        // UI更新
        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}