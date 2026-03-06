using System.Collections.Generic;
using UnityEngine;
using Vosk;

public class SpeechToText : MonoBehaviour
{
    [SerializeField] private VoiceProcessor _voiceProcessor;
    [SerializeField] private float _recordingSampleRate = 16000.0f;
    [SerializeField] private List<string> _keyPhrases = new();

    private VoskModel _model;
    private VoskRecognizer _rec;

    private bool _running = false;
    private string _grammar = "";

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

    private void Start()
    {
        _model = VoskModel.Instance;
        UpdateGrammar();
    }

    private void ProcessAudioFrame(short[] data)
    {
        if (!_running || !_model.ModelReady || _rec == null)
            return;

        if (_rec.AcceptWaveform(data, data.Length))
        {
            var result = _rec.Result();

            if (ContainsKeyPhrase(result))
            {
                Debug.Log("💖 KYUN DETECTED 💖");
                StopRecording();
            }
        }
        else
        {
            var partial = _rec.PartialResult();

            if (ContainsKeyPhrase(partial))
            {
                Debug.Log("💖 KYUN DETECTED (partial) 💖");
                StopRecording();
            }
        }
    }

    private void OnRecordingStop()
    {
        Debug.Log("Recording stopped.");
    }

    public void StartRecording()
    {
        if (_running || !_model.ModelReady)
            return;

        _running = true;

        if (string.IsNullOrEmpty(_grammar))
            _rec = new VoskRecognizer(_model.Model, _recordingSampleRate);
        else
            _rec = new VoskRecognizer(_model.Model, _recordingSampleRate, _grammar);

        _voiceProcessor.StartRecording();

        Debug.Log("🎤 Started recording");
    }

    public void StopRecording()
    {
        if (!_running)
            return;

        _running = false;

        _voiceProcessor.StopRecording();

        _rec?.Dispose();
        _rec = null;

        Debug.Log("🛑 Stopped recording");
    }

    private void UpdateGrammar()
    {
        if (_keyPhrases.Count == 0)
        {
            _grammar = "";
            return;
        }

        List<string> phrases = new();

        foreach (string keyphrase in _keyPhrases)
            phrases.Add($"\"{keyphrase}\"");

        // phrases.Add("\"[unk]\"");

        _grammar = "[" + string.Join(",", phrases) + "]";
    }

    private bool ContainsKeyPhrase(string result)
    {
        if (string.IsNullOrEmpty(result))
            return false;

        string lower = result.ToLower();

        foreach (var phrase in _keyPhrases)
        {
            if (lower.Contains(phrase))
                return true;
        }

        return false;
    }
}