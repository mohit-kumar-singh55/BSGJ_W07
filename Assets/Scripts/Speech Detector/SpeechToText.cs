using UnityEngine;
using Vosk;

namespace BSGJ_W07
{
    public class SpeechToText : MonoBehaviour
    {
        void Start()
        {
            var model = new Model("vosk-model-small-ja-0.22");
            var rec = new VoskRecognizer(model, 16000.0f);
            // rec.AcceptWaveform("path/to/audio/file.wav");
            // var result = rec.Result();
        }
    }
}