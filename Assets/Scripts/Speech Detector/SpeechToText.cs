using System.Collections.Generic;
using UnityEngine;
using Vosk;

[RequireComponent(typeof(VoiceProcessor))]
public class SpeechToText : MonoBehaviour
{
    [SerializeField] private VoiceProcessor _voiceProcessor;
    [SerializeField] private float _recordingSampleRate = 16000.0f;

    private VoskModel _model;
    private VoskRecognizer _rec;

    private bool _running = false;
    private bool _phraseDetected = false;
    private string _grammar = "";
    private List<string> _keyPhrases = new();

    // set key phrases and update grammar accordingly
    public List<string> KeyPhrases { set { _keyPhrases = value; UpdateGrammar(); } }

    public event System.Action<string, int> OnKeyPhraseDetected;
    public event System.Action<string, int> OnKeyPhraseUnDetected;

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
        // init
        _model = VoskModel.Instance;
    }

    private void ProcessAudioFrame(short[] data)
    {
        if (!_running || !_model.ModelReady || _rec == null)
            return;

        string result;

        if (_rec.AcceptWaveform(data, data.Length))

            result = _rec.Result();
        else
            result = _rec.PartialResult();

        if (ContainsKeyPhrase(result))
        {
            _phraseDetected = true;
            OnKeyPhraseDetected?.Invoke(result, NormalizedVolume());
            Debug.Log("💖 KYUN DETECTED 💖");
            CancelInvoke(nameof(StopRecording));   // cancel the auto stop if key phrase detected
            StopRecording();
        }
    }

    private void OnRecordingStop()
    {
        Debug.Log("Recording stopped.");
    }

    public void StartRecording(float duration = 3f)
    {
        if (_running || !_model.ModelReady)
            return;

        _running = true;
        _phraseDetected = false;

        if (string.IsNullOrEmpty(_grammar))
            _rec = new VoskRecognizer(_model.Model, _recordingSampleRate);
        else
            _rec = new VoskRecognizer(_model.Model, _recordingSampleRate, _grammar);

        _voiceProcessor.StartRecording();

        Debug.Log("🎤 Started recording");

        // stop recording after specified duration
        Invoke(nameof(StopRecording), duration);
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

        // when recording stops without detecting key phrase by timeout, invoke un-detected event
        if (!_phraseDetected) OnKeyPhraseUnDetected?.Invoke("Not detected & Recording stopped because of timeout", NormalizedVolume());
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

    private int NormalizedVolume() => Mathf.RoundToInt(Mathf.Clamp01(_voiceProcessor.MaxVolume) * 100);
}