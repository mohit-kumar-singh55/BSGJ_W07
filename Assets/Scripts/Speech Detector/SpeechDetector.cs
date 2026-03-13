using System.Collections.Generic;
using UnityEngine;

// it starts the speech detection process, checks the timing of the speech we are looking for, and score based on the timing accuracy
[RequireComponent(typeof(SpeechToText))]
public class SpeechDetector : MonoBehaviour
{
    [Tooltip("Time window (in seconds), if speech detected in-between this time, will be considered as Good. Should be in format (min, max) where min < max")]
    [SerializeField] private Vector2 _detectionWindow = new(2f, 3f);
    [SerializeField] private List<string> _keyPhrases = new();

    private float _startTime;
    private SpeechToText _speechToText;
    private enum DetectionStage { Early = 80, Good = 100, Late = 60, Miss = 50 }

    public event System.Action<int, string> OnRecordingCompleted = delegate { };

    void OnDestroy()
    {
        _speechToText.OnKeyPhraseDetected -= OnKeyPhraseDetected;
        _speechToText.OnKeyPhraseUnDetected -= OnKeyPhraseUnDetected;
    }

    private void Start()
    {
        _speechToText = FindAnyObjectByType<SpeechToText>();
        if (_speechToText == null)
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
    }

    public void StartDetection()
    {
        // start rec
        _speechToText.StartRecording(_detectionWindow.y);

        // record the start time
        _startTime = Time.time;
    }

    private void OnKeyPhraseDetected(string keyphrase)
    {
        float elapsedTime = Time.time - _startTime;
        // Debug.Log($"Key phrase detected: {keyphrase}, Elapsed Time: {elapsedTime} seconds");

        if (elapsedTime < _detectionWindow.x)
        {
            //  score as Early
            OnRecordingCompleted?.Invoke((int)DetectionStage.Early, keyphrase);
        }
        else if (elapsedTime <= _detectionWindow.y)
        {
            // score as Good
            OnRecordingCompleted?.Invoke((int)DetectionStage.Good, keyphrase);
        }
        else
        {
            // score as Late
            OnRecordingCompleted?.Invoke((int)DetectionStage.Late, keyphrase);
        }
    }

    private void OnKeyPhraseUnDetected(string message)
    {
        // score as Miss
        OnRecordingCompleted?.Invoke((int)DetectionStage.Miss, message + " Score: Miss");
    }
}