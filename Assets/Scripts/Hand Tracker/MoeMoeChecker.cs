using Mediapipe.Tasks.Vision.HandLandmarker;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using UnityEngine;

public class MoeMoeChecker : MonoBehaviour
{
    // レシーバー
    [SerializeField] private HandDataReceiver receiver;
    [SerializeField] private HandLandmarkerRunner handLandmarker;

    // 萌え萌えきゅんの進行度
    [SerializeField] private int phase;

    // 前フレームのX移動距離
    private float prevHandMoveDirX;

    // 前のフェーズからの手の移動距離X
    private float HandMoveDirX;
    // 前のフェーズからの手の移動距離X
    private float HandMoveDirY;

    // 最初の手と手の距離
    private float startHandDistance;

    // 次のフェーズの移動方向
    private int nextMoveDir;

    void Start()
    {
        Initalize();
        EndCheck();
    }

    private void Initalize()
    {
        // 初期化
        phase = 1;
        startHandDistance = 0;
        HandMoveDirX = 0;
        HandMoveDirY = 0;
        prevHandMoveDirX = 0;
        nextMoveDir = 0;
    }

    private void FixedUpdate()
    {
        switch (phase)
        {
            case 0: break;
            case 1: Phase1(); break;
            case 2: Phase2(); break;
            case 3: Phase3(); break;
            case 4: Phase4(); break;
        }
    }

    // チェック開始
    public void StartCheck()
    {
        Initalize();
        receiver.gameObject.SetActive(true);
        handLandmarker.gameObject.SetActive(true);
    }

    // チェック終了
    public void EndCheck()
    {
        Initalize();
        receiver.gameObject.SetActive(false);
        handLandmarker.gameObject.SetActive(false);
    }

    // フェーズ1
    private void Phase1()
    {
        // 手が2つあるかのチェック
        if (!receiver.AreTwoHandsPresent(receiver.result))
        {
            Debug.Log("手が2つ無い");
            return;
        }

        // 手の形がハートになっているかのチェック
        if (receiver.AreAllFingertipsHigherThanBase(receiver.result) || receiver.AreFingertipsBentInward(receiver.result) || receiver.IsThumbTipHighest(receiver.result))
        {
            Debug.Log("手の形がハートではない");

            return;
        }

        // 手がしばらく止まっている
        if (receiver.isFleezCount[0] > 5 && receiver.isFleezCount[1] > 5)
        {


            // フェーズ移行処理
            SetStartHandDistance();

            HandMoveDirX = 0;
            HandMoveDirY = 0;
            nextMoveDir = 0;

            phase = 2;
            Debug.Log("萌え萌えキュンセット");
        }
    }

    // フェーズ2
    private void Phase2()
    {
        // 手の距離のチェック
        if (HandDistanceCheck()) MoveStart();

        // 手の移動の更新
        if (receiver.handMoveDir[0].x != prevHandMoveDirX)
        {
            prevHandMoveDirX = receiver.handMoveDir[0].x;
            HandMoveDirX += receiver.handMoveDir[0].x;
            HandMoveDirY += receiver.handMoveDir[0].y;
        }

        // 一定量横に移動したかのチェック
        if (Mathf.Abs(HandMoveDirX) > 0.09f)
        {
            nextMoveDir = HandMoveDirX > 0 ? -1 : 1;
            HandMoveDirX = 0;
            HandMoveDirY = 0;
            phase = 3;
            Debug.Log("萌え");
        }

        // 縦方向に移動しすぎていないかのチェック
        if (Mathf.Abs(HandMoveDirY) > 0.1f)
        {
            MoveStart();
        }
    }

    private void Phase3()
    {
        // 手の距離のチェック
        if (HandDistanceCheck()) MoveStart();

        // 手の移動の更新
        if (receiver.handMoveDir[0].x != prevHandMoveDirX)
        {
            prevHandMoveDirX = receiver.handMoveDir[0].x;
            HandMoveDirX += receiver.handMoveDir[0].x;
            HandMoveDirY += receiver.handMoveDir[0].y;
        }

        // 一定量横に移動したかのチェック
        if (HandMoveDirX * nextMoveDir > 0.09f)
        {
            nextMoveDir *= -1;
            HandMoveDirX = 0;
            HandMoveDirY = 0;
            phase = 4;
            Debug.Log("萌え");
        }

        // 縦方向に移動しすぎていないかのチェック
        if (Mathf.Abs(HandMoveDirY) > 0.1f)
        {
            MoveStart();
        }
    }

    private void Phase4()
    {
        // 手の距離のチェック
        if (HandDistanceCheck()) phase = 1;

        // 手の移動の更新
        if (receiver.handMoveDir[0].x != prevHandMoveDirX)
        {
            prevHandMoveDirX = receiver.handMoveDir[0].x;
            HandMoveDirX += receiver.handMoveDir[0].x;
            HandMoveDirY += receiver.handMoveDir[0].y;
        }

        // 一定量横に移動したかのチェック
        if (HandMoveDirX * nextMoveDir > 0.04f)
        {
            MoveStart();
            Debug.Log("キュン");
            Debug.Log("萌え萌えキュン成功！");
        }

        // 縦方向に移動しすぎていないかのチェック
        if (Mathf.Abs(HandMoveDirY) > 0.1f)
        {
            MoveStart();
        }
    }

    // 初期化
    private void MoveStart()
    {
        nextMoveDir = 0;
        HandMoveDirX = 0;
        HandMoveDirY = 0;
        phase = 1;
    }

    // セッター：手の距離
    private void SetStartHandDistance()
    {
        startHandDistance = receiver.handsDistance;
    }

    // 手の距離のチェック
    private bool HandDistanceCheck()
    {
        return startHandDistance > receiver.handsDistance + 0.1 || startHandDistance < receiver.handsDistance - 0.1;
    }


}