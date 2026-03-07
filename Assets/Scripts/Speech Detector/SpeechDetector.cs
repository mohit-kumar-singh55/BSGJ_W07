using System.Collections.Generic;
using UnityEngine;

// it starts the speech detection process, checks the timing of the speech we are looking for, and score based on the timing accuracy
public class SpeechDetector : MonoBehaviour
{
    [Tooltip("Time window (in seconds), if speech detected in-between this time, will be considered as Good. Should be in format (min, max) where min < max")]
    [SerializeField] private Vector2 _detectionWindow = new(2f, 3f);
    [SerializeField] private List<string> _keyPhrases = new();

    // ! temp
    [SerializeField] private GameObject _detectionVFXPrefab;

    private float _startTime;
    private VFXCountdown _vfxCountdown;
    private SpeechToText _speechToText;

    void OnDestroy()
    {
        _speechToText.OnKeyPhraseDetected -= OnKeyPhraseDetected;
        _speechToText.OnKeyPhraseUnDetected -= OnKeyPhraseUnDetected;
    }

    private void Start()
    {
        // ! temporary: just for testing
        _vfxCountdown = FindAnyObjectByType<VFXCountdown>();
        _speechToText = FindAnyObjectByType<SpeechToText>();
        if (_vfxCountdown == null || _speechToText == null)
        {
            Debug.LogError("One or more required components not found in the scene. Please add them.");
            enabled = false;
            return;
        }

        // set key phrases to search for
        _speechToText.KeyPhrases = _keyPhrases;

        // bind to get know whether key phrase is detected or not
        _speechToText.OnKeyPhraseDetected += OnKeyPhraseDetected;
        _speechToText.OnKeyPhraseUnDetected += OnKeyPhraseUnDetected;

        // ! temporary: just for testing
        StartDetection();
    }

    public void StartDetection()
    {
        // ! temp: wait/show countdown before starting detection
        _vfxCountdown.CountdownInterval = 1f;   // time between countdowns

        // start countdown
        _vfxCountdown.StartCountdown(() =>
        {
            // start rec
            _speechToText.StartRecording(_detectionWindow.y);

            // record the start time
            _startTime = Time.time;
        });
    }

    private void OnKeyPhraseDetected(string keyphrase)
    {
        float elapsedTime = Time.time - _startTime;
        Debug.Log($"Key phrase detected: {keyphrase}, Elapsed Time: {elapsedTime} seconds");

        if (elapsedTime < _detectionWindow.x)
        {
            // TODO: score as Early
            Debug.Log("Too early! Score: Early");
        }
        else if (elapsedTime <= _detectionWindow.y)
        {
            // TODO: score as Good
            Debug.Log("Good timing! Score: Good");
        }
        else
        {
            // TODO: score as Late
            Debug.Log("Too late! Score: Late");
        }

        // ! temp
        if (_detectionVFXPrefab != null) Instantiate(_detectionVFXPrefab, new(.6f, -5.48f, 1.17947f), Quaternion.identity);
    }

    private void OnKeyPhraseUnDetected(string message)
    {
        // TODO: score as Miss
        Debug.Log(message + " Score: Miss");
    }
}