using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using UnityEngine;

public class HandDataReceiver : MonoBehaviour
{
    [SerializeField] private HandLandmarkerRunner runner;

    [SerializeField] private int numHands;

    private int phase;
    private void Start()
    {
        runner.OnHandResultOutput += OnHandResult;
        runner.config.NumHands = numHands;
    }

    private void Update()
    {
        
    }

    private void OnHandResult(HandLandmarkerResult result)
    {
        if (result.handLandmarks == null || result.handLandmarks.Count == 0)
            return;

        var wrist1 = result.handLandmarks[0].landmarks[0];
    }
}