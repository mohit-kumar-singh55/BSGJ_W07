using Mediapipe.Unity.Sample.HandLandmarkDetection;
using UnityEngine;

public class MoeMoeChecker2 : MonoBehaviour
{
    // コメントアウトをするフレーム
    private const int TimeToCheck = 0;
    // チェックする時間
    private float[] checkTime;

    // 今のチェック時間
    private float nowCheckTime = 0f;

    [SerializeField] private int point;

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

    // クリアしたフェーズ
    private int clearPhase;

    private bool isInitializing = true;
    private float initializeTimer = 0;
    private float initializeUpTo = .5f;

    private

    void Start()
    {
        isInitializing = true;
        initializeTimer = 0;
        StartCheck();
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
        point = 0;
        checkTime = new float[4] { 0, 1, 1, 1 };
    }

    // チェック開始
    public void StartCheck()
    {
        Initalize();
        receiver.gameObject.SetActive(true);
        StartCoroutine(handLandmarker.Resume());
    }

    // チェック終了
    public void EndCheck()
    {
        Initalize();
        receiver.gameObject.SetActive(false);
        handLandmarker.Pause();
    }

    private void FixedUpdate()
    {
        if (isInitializing)
        {
            if (initializeTimer > initializeUpTo)
            {
                EndCheck();
                isInitializing = false;
            }
            else initializeTimer += Time.fixedDeltaTime;
        }
        else
        {
            switch (phase)
            {
                case 1: Phase1(); break;
                case 2: Phase2(); break;
                case 3: Phase3(); break;
                case 4: Phase4(); break;
                default: break;
            }
            nowCheckTime += Time.fixedDeltaTime;
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
        if (receiver.isFleezCount[0] > 2 && receiver.isFleezCount[1] > 2)
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

        // 判定のチェック
        if (nowCheckTime >= checkTime[phase - 1])
        {
            // 一定量横に移動したかのチェック
            if (Mathf.Abs(HandMoveDirX) > 0.09f) point++;
            NextPhase();
            Debug.Log("萌え");
            nextMoveDir = HandMoveDirX > 0 ? -1 : 1;
        }
    }

    private void Phase3()
    {
        // 手の距離のチェック
        if (HandDistanceCheck()) MoveStart();

        // 手の移動の更新
        UpdateMoveDir();

        // 判定のチェック
        if (nowCheckTime >= checkTime[phase - 1])
        {
            // 一定量横に移動したかのチェック
            if (Mathf.Abs(HandMoveDirX) > 0.09f) point++;
            NextPhase();
            Debug.Log("萌え");
        }
    }

    private void Phase4()
    {
        // 手の距離のチェック
        if (HandDistanceCheck()) EndCheck();

        // 手の移動の更新
        UpdateMoveDir();

        // 判定のチェック
        if (nowCheckTime >= checkTime[phase - 1])
        {
            // 一定量横に移動したかのチェック
            if (Mathf.Abs(HandMoveDirX) > 0.04f) point++;
            Debug.Log("キュン");
            EndCheck();
            return;
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
        if (receiver.handMoveDir[0].x == prevHandMoveDirX) return false;
        prevHandMoveDirX = receiver.handMoveDir[0].x;
        HandMoveDirX += receiver.handMoveDir[0].x;
        HandMoveDirY += receiver.handMoveDir[0].y;
        return true;
    }
}