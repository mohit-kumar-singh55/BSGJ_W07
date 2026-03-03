using System.IO;
using UnityEngine;
using Vosk;

public class SpeechToText : MonoBehaviour
{
    [SerializeField] private VoiceProcessor _voiceProcessor;
    // [SerializeField] private float _recordingDuration = 30f;
    [SerializeField] private float _recordingSampleRate = 16000.0f;

    private Model _model;
    private VoskRecognizer _rec;
    private string _fullModelPath;
    private bool _running = false;
    private string _result = "";

    private const string _modelPath = "vosk-model-ja-0.22";

    public string Result => _result;

    private void OnEnable()
    {
        _voiceProcessor.OnFrameCaptured += ProcessAudioFrame;
        _voiceProcessor.OnRecordingStop += OnRecordingStop;
    }

    private void OnDisable()
    {
        _voiceProcessor.OnFrameCaptured -= ProcessAudioFrame;
        _voiceProcessor.OnRecordingStop -= OnRecordingStop;
    }

    private void OnDestroy() => CleanUp();

    private void Start()
    {
        _fullModelPath = Path.Combine(Application.streamingAssetsPath, _modelPath).Replace("\\", "/");
    }

    private void ProcessAudioFrame(short[] data)
    {
        if (_rec != null && _rec.AcceptWaveform(data, data.Length))
        {
            string result = _rec.Result();
            _result += (result + "").Trim();
            Debug.Log("Result: " + result);
        }
    }

    private void OnRecordingStop()
    {
        string partialResult = _rec?.PartialResult();
        _result += (partialResult + "").Trim();
        Debug.Log("Partial Result: " + partialResult);
    }

    private void CleanUp()
    {
        _rec?.Dispose();
        _model?.Dispose();
        _rec = null;
        _model = null;
    }

    public void StartRecording()
    {
        if (_running) return;

        // Initialize Vosk model and recognizer
        _running = true;
        _model = new Model(_fullModelPath);
        _rec = new VoskRecognizer(_model, _recordingSampleRate);

        // Start recording
        _voiceProcessor.StartRecording();
        Debug.Log("*********** Started recording ***********");
    }

    public void StopRecording()
    {
        if (!_running) return;

        // Stop recording and processing
        _running = false;
        _voiceProcessor.StopRecording();
        Debug.Log("*********** Stopped recording ***********");

        // clean up resources
        CleanUp();
    }
}