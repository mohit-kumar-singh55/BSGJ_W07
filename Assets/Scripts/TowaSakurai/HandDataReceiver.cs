using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using UnityEngine;

public class HandDataReceiver : MonoBehaviour
{
    [SerializeField] private HandLandmarkerRunner runner;
    [SerializeField] private int numHands;
    [SerializeField] private int prevPosUpdateCountMax;

    [SerializeField]private int[] isFleezCount;
    private float handsDistance;
    private int prevPosUpdateCount;

    private Vector3[] handPos;
    private Vector3[] prevHandPos;

    private void Start()
    {
        runner.OnHandResultOutput += OnHandResult;
        runner.config.NumHands = numHands;

        isFleezCount = new int[numHands];
        handPos = new Vector3[numHands];
        prevHandPos = new Vector3[numHands];

        for (int i = 0; i < numHands; i++)
        {
            handPos[i] = Vector3.zero;
            prevHandPos[i] = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        prevPosUpdateCount++;

        if (prevPosUpdateCount > prevPosUpdateCountMax)
        {
            IsSlowHand(0);
            IsSlowHand(1);

            UpdatePrevHands();

            prevPosUpdateCount = 0;
        }
    }

    private void OnHandResult(HandLandmarkerResult result)
    {
        if (result.handLandmarks == null || result.handLandmarks.Count == 0)
            return;

        int count = Mathf.Min(result.handLandmarks.Count, numHands);

        for (int i = 0; i < count; i++)
        {
            var wrist = result.handLandmarks[i].landmarks[0];
            handPos[i] = new Vector3(wrist.x, wrist.y, wrist.z);
        }
    }

    private void IsSlowHand(int index)
    {
        if (IsHandMovingSlow(index, 0.01f))
        {
            isFleezCount[index]++;
        }
        else
        {
            isFleezCount[index] = 0;
        }
    }

    private void UpdatePrevHands()
    {
        for (int i = 0; i < numHands; i++)
        {
            prevHandPos[i] = handPos[i];
        }
    }

    private bool IsHandMovingSlow(int index, float threshold)
    {
        if (index < 0 || index >= handPos.Length)
            return false;

        float distance = Vector3.Distance(handPos[index], prevHandPos[index]);

        return distance <= threshold;
    }

}