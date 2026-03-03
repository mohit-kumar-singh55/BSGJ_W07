using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using UnityEngine;

public class HandDataReceiver : MonoBehaviour
{
    [SerializeField] private HandLandmarkerRunner runner;

    private void Start()
    {
        runner.OnHandResultOutput += OnHandResult;
    }

    private void OnHandResult(HandLandmarkerResult result)
    {
        if (result.handLandmarks == null || result.handLandmarks.Count == 0)
            return;

        var wrist = result.handLandmarks[0].landmarks[0];
        float x = wrist.x;
        float y = wrist.y;
        float z = wrist.z;

        Debug.Log($"ŽèŽñ: x={x}, y={y}, z={z}");
    }
}