using Mediapipe.Tasks.Vision.HandLandmarker;
using UnityEngine;

public class MoeMoeChecker2 : MonoBehaviour
{
    // チェックする時間
    [SerializeField] private int[] checkTime;

    // 今のチェック時間
    [SerializeField] private int nowCheckTime;

    [SerializeField] private int point;

    // レシーバー
    [SerializeField] private HandDataReceiver receiver;

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

    // クリアしたフェーズ
    private int clearPhase;

    void Start()
    {
        // 初期化
        phase = 1;
        startHandDistance = 0;
        HandMoveDirX = 0;
        HandMoveDirY = 0;
        prevHandMoveDirX = 0;
        nextMoveDir = 0;
        point = 0;
        checkTime = new int[5] {0,0,80,80,80};
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

    // フェーズ1
    private void Phase1()
    {
        // 手が2つあるかのチェック
        if (!receiver.AreTwoHandsPresent(receiver.result))
        {
            //Debug.Log("手が2つ無い");
            return;
        }

        // 手の形がハートになっているかのチェック
        if (receiver.AreAllFingertipsHigherThanBase(receiver.result) || receiver.AreFingertipsBentInward(receiver.result) || receiver.IsThumbTipHighest(receiver.result))
        {
           // Debug.Log("手の形がハートではない");

            return;
        }

        // 手がしばらく止まっている
        if (receiver.isFleezCount[0] > 5 && receiver.isFleezCount[1] > 5)
        {
            // フェーズ移行処理
            SetStartHandDistance();

            nextMoveDir = 0;
            point = 0;
            NextPhase();
            Debug.Log("チェックスタート");
        }
    }

    // フェーズ2
    private void Phase2()
    {
        // 手の距離のチェック
        if (HandDistanceCheck()) MoveStart();

        // 手の移動の更新
        UpdateMoveDir();

        // タイムのチェック
        if (nowCheckTime == 20)
        {
            Debug.Log("萌え");
        }

        // 判定のチェック
        if(nowCheckTime == checkTime[phase])
        {
            // 一定量横に移動したかのチェック
            if (Mathf.Abs(HandMoveDirX) > 0.09f)
            {
                point++;
            }
            NextPhase();
            nextMoveDir = HandMoveDirX > 0 ? -1 : 1;
        }

        nowCheckTime++;
    }

    private void Phase3()
    {
        // 手の距離のチェック
        if (HandDistanceCheck()) MoveStart();

        // 手の移動の更新
        UpdateMoveDir();

        // タイムのチェック
        if (nowCheckTime == 20)
        {
            Debug.Log("萌え");
        }

        // 判定のチェック
        if (nowCheckTime == checkTime[phase])
        {
            // 一定量横に移動したかのチェック
            if (Mathf.Abs(HandMoveDirX) > 0.09f)
            {
                point++;
            }
            NextPhase();
        }

        nowCheckTime++;
    }

    private void Phase4()
    {
        // 手の距離のチェック
        if (HandDistanceCheck()) phase = 1;

        // 手の移動の更新
        UpdateMoveDir();
        // タイムのチェック
        if (nowCheckTime == 20)
        {
            Debug.Log("キュン");
        }

        // 判定のチェック
        if (nowCheckTime == checkTime[phase])
        {
            // 一定量横に移動したかのチェック
            if (Mathf.Abs(HandMoveDirX) > 0.04f)
            {
                point++;
            }
            MoveStart();
        }

        nowCheckTime++;
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

    private void NextPhase()
    {
        nowCheckTime = 0;
        nextMoveDir *= -1;
        HandMoveDirX = 0;
        HandMoveDirY = 0;
        phase++;
    }

    private bool UpdateMoveDir()
    {
        if (receiver.handMoveDir[0].x == prevHandMoveDirX)return false;
        prevHandMoveDirX = receiver.handMoveDir[0].x;
        HandMoveDirX += receiver.handMoveDir[0].x;
        HandMoveDirY += receiver.handMoveDir[0].y;
        return true;
    }


}