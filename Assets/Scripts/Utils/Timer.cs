using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private float _timeLimit = 90f;

    // ! TEMP: separate it into a ui class
    [SerializeField] private GameObject _needleUI;
    [SerializeField] private float _rotationOffset = -211f;     // current rotation of needle ui

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
        // _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        RotateNeedle();
    }

    // ! TEMP: separate it into a ui class
    private void RotateNeedle()
    {
        float t = Mathf.Clamp01(currentTime / _timeLimit);
        float angle = t * -360f + _rotationOffset;
        _needleUI.transform.localEulerAngles = new Vector3(0, 0, angle);
    }
}