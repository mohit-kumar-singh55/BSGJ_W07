using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using UnityEngine;

public class HandDataReceiver : MonoBehaviour
{
    private int prevPosUpdateCount;

    [SerializeField] private HandLandmarkerRunner runner;
    [SerializeField] private int numHands;
    [SerializeField] private int prevPosUpdateCountMax;

    // 止まり続けたフレーム
    public int[] isFleezCount { get;private set; }
    // 手と手の距離
    public float handsDistance { get;private set; }
    // 手の移動距離と移動方向
    public Vector2[] handMoveDir { get;private set; }
    // 手の座標
    public Vector3[] handPos { get;private set; }
    // １プレーム前の
    public Vector3[] prevHandPos{ get;private set; }

    private void Start()
    {
        runner.OnHandResultOutput += OnHandResult;
        runner.config.NumHands = numHands;

        isFleezCount = new int[numHands];
        handPos = new Vector3[numHands];
        prevHandPos = new Vector3[numHands];
        handMoveDir = new Vector2[numHands];

        for (int i = 0; i < numHands; i++)
        {
            handPos[i] = Vector3.zero;
            prevHandPos[i] = Vector3.zero;
            handMoveDir[i] = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        prevPosUpdateCount++;

        if (prevPosUpdateCount > prevPosUpdateCountMax)
        {
            IsSlowHand(0);
            IsSlowHand(1);

            handsDistance = Distance(handPos[0], handPos[1]);

            handMoveDir[0] = CheckMoveDir(handPos[0], prevHandPos[0]);
            handMoveDir[1] = CheckMoveDir(handPos[1], prevHandPos[1]);

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

    private float Distance(Vector3 a,Vector3 b)
    {
        return Mathf.Sqrt(
            (a.x - b.x) * (a.x - b.x) +
            (a.y - b.y) * (a.y - b.y) +
            (a.z - b.z) * (a.z - b.z)
        );
    }

    private Vector2 CheckMoveDir(Vector3 a, Vector3 b)
    {
        Vector2 moveDir = Vector2.zero;

        Vector3 diff = b - a;

            moveDir.x = diff.x;

            moveDir.y = diff.y;

        return moveDir;
    }
}