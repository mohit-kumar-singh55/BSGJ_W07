using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using UnityEngine;

public class HandDataReceiver : MonoBehaviour
{
    // prevPosをアップデートするまでのカウント
    private int prevPosUpdateCount;

    // ランナー
    [SerializeField] private HandLandmarkerRunner runner;

    // 手の数（基本は２）
    [SerializeField] private int numHands;

    // prevPosのこうしんひんど
    [SerializeField] private int prevPosUpdateCountMax;

    // リザルト
    public HandLandmarkerResult result { get; private set; }

    // 止まり続けたフレーム
    public int[] isFleezCount { get; private set; }

    // 手と手の距離
    public float handsDistance { get; private set; }

    // 手の移動距離と移動方向
    public Vector2[] handMoveDir { get; private set; }

    // 手の座標
    public Vector3[] handPos { get; private set; }

    // １プレーム前の
    public Vector3[] prevHandPos { get; private set; }

    private void Start()
    {
        //初期化
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

        //更新科のチェック
        if (prevPosUpdateCount > prevPosUpdateCountMax)
        {
            // prevPosの更新
            IsSlowHand(0);
            IsSlowHand(1);

            handsDistance = Distance(handPos[0], handPos[1]);

            handMoveDir[0] = CheckMoveDir(handPos[0], prevHandPos[0]);
            handMoveDir[1] = CheckMoveDir(handPos[1], prevHandPos[1]);

            UpdatePrevHands();

            prevPosUpdateCount = 0;
        }
    }

    /// <summary>
    /// 手のリザルト
    /// </summary>
    /// <param name="result">リザルト</param>
    private void OnHandResult(HandLandmarkerResult result)
    {
        this.result = result;

        if (result.handLandmarks == null || result.handLandmarks.Count == 0)
            return;

        int count = Mathf.Min(result.handLandmarks.Count, numHands);

        for (int i = 0; i < count; i++)
        {
            var wrist = result.handLandmarks[i].landmarks[0];
            handPos[i] = new Vector3(wrist.x, wrist.y, wrist.z);
        }
    }

    /// <summary>
    /// 手が止まっているかのチェック
    /// </summary>
    /// <param name="index">手の番号</param>
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

    // prevHandsのアップデート
    private void UpdatePrevHands()
    {
        for (int i = 0; i < numHands; i++)
        {
            prevHandPos[i] = handPos[i];
        }
    }

    /// <summary>
    /// 速度がtgresholdより遅いかのチェック
    /// </summary>
    /// <param name="index">手の番号</param>
    /// <param name="threshold">速度</param>
    /// <returns></returns>
    private bool IsHandMovingSlow(int index, float threshold)
    {
        if (index < 0 || index >= handPos.Length)
            return false;

        float distance = Vector3.Distance(handPos[index], prevHandPos[index]);

        return distance <= threshold;
    }

    /// <summary>
    /// 距離を測る関数
    /// </summary>
    /// <param name="a">オブジェクトA</param>
    /// <param name="b">オブジェクトB</param>
    /// <returns></returns>
    private float Distance(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt(
            (a.x - b.x) * (a.x - b.x) +
            (a.y - b.y) * (a.y - b.y) +
            (a.z - b.z) * (a.z - b.z)
        );
    }

    /// <summary>
    /// 移動方向のチェック
    /// </summary>
    /// <param name="a">オブジェクトA</param>
    /// <param name="b">オブジェクトB</param>
    /// <returns></returns>
    private Vector2 CheckMoveDir(Vector3 a, Vector3 b)
    {
        Vector2 moveDir = Vector2.zero;

        Vector3 diff = b - a;

        moveDir.x = diff.x;

        moveDir.y = diff.y;

        return moveDir;
    }

    /// <summary>
    /// 全ての指が根本より下がっているかのチェック
    /// </summary>
    /// <param name="result">リザルト</param>
    /// <returns>下がっていればtrue</returns>
    public bool AreAllFingertipsHigherThanBase(HandLandmarkerResult result)
    {
        if (result.handLandmarks == null || result.handLandmarks.Count == 0)
            return false;

        int[,] fingerPairs = new int[,]
        {
        { 2, 4 },   // 親指
        { 5, 8 },   // 人差し指
        { 9, 12 },  // 中指
        { 13, 16 }, // 薬指
        { 17, 20 }  // 小指
        };

        int handCount = result.handLandmarks.Count;

        for (int h = 0; h < handCount; h++)
        {
            var lm = result.handLandmarks[h].landmarks;

            for (int i = 0; i < fingerPairs.GetLength(0); i++)
            {
                int baseId = fingerPairs[i, 0];
                int tipId = fingerPairs[i, 1];

                float baseY = lm[baseId].y;
                float tipY = lm[tipId].y;

                if (tipY >= baseY)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 全ての指が中心に向かっているかのチェック
    /// </summary>
    /// <param name="result">リザルト</param>
    /// <returns>中心に向かっていればtrue</returns>
    public bool AreFingertipsBentInward(HandLandmarkerResult result)
    {
        if (result.handLandmarks == null || result.handLandmarks.Count == 0)
            return false;

        int[,] fingerPairs = new int[,]
        {
        { 2, 4 },   // 親指
        { 5, 8 },   // 人差し指
        { 9, 12 },  // 中指
        { 13, 16 }, // 薬指
        { 17, 20 }  // 小指
        };

        int handCount = result.handLandmarks.Count;
        
        for (int h = 0; h < handCount; h++)
        {
            var lm = result.handLandmarks[h].landmarks;

            float direction = (result.handedness[h].categories[0].categoryName == "Right") ? 1f : -1f;

            for (int i = 0; i < fingerPairs.GetLength(0); i++)
            {
                int baseId = fingerPairs[i, 0];
                int tipId = fingerPairs[i, 1];

                float baseX = lm[baseId].x * direction;
                float tipX = lm[tipId].x * direction;

                if (tipX >= baseX)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 親指が全ての指の中で一番低いかのチェック
    /// </summary>
    /// <param name="result">リザルト</param>
    /// <returns>低ければtrue</returns>
    public bool IsThumbTipHighest(HandLandmarkerResult result)
    {
        if (result.handLandmarks == null || result.handLandmarks.Count == 0)
            return false;

        // 親指の先端は landmark 4
        const int thumbTip = 4;

        for (int h = 0; h < result.handLandmarks.Count; h++)
        {
            var lm = result.handLandmarks[h].landmarks;

            float thumbY = lm[thumbTip].y;

            // すべての指先（tip）をチェック
            int[] fingertipIds = { 4, 8, 12, 16, 20 };

            foreach (int tipId in fingertipIds)
            {
                if (tipId == thumbTip) continue;

                float tipY = lm[tipId].y;

                if (thumbY >= tipY)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 手が2つあるかのチェック
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public bool AreTwoHandsPresent(HandLandmarkerResult result)
    {
        return result.handLandmarks != null && result.handLandmarks.Count >= 2;
    }
}