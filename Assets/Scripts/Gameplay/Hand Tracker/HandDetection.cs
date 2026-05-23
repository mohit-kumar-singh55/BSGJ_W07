using Mediapipe.Unity.Sample.HandLandmarkDetection;
using UnityEngine;

public class HandDetection : MonoBehaviour
{
    // チェックする時間
    private float[] checkTime;

    // 今のチェック時間
    [SerializeField] private float nowCheckTime = 0f;

    [SerializeField] private int point;

    // レシーバー
    [SerializeField] private HandDataReceiver receiver;

    [SerializeField] private HandLandmarkerRunner handLandmarker;

    // 萌え萌えきゅんの進行度
    [SerializeField] private int phase;

    // 前フレームのX移動距離
    [SerializeField] private float prevHandMoveDirX;

    // 前のフェーズからの手の移動距離X
    private float HandMoveDirX;

    // 前のフェーズからの手の移動距離X
    private float HandMoveDirY;

    // 最初の手と手の距離
    private float startHandDistance;

    private bool isInitializing = true;
    private float initializeTimer = 0;
    private float initializeUpTo = .5f;

    private bool isDetecting = false;

    // 手がハートになってからのカウント
    public float HeartTimeCount;

    public event System.Action OnHandCheckStart = delegate { };
    public event System.Action<int> OnHandDetectionProceed = delegate { }; // phase
    public event System.Action<int> OnHandDetectionOver = delegate { }; // point

    private void Start()
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
        point = 0;
        checkTime = new float[4] { 0, 1, 1, 1 };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCheck();
        }
    }

    // チェック開始
    public void StartCheck()
    {
        Initalize();
        nowCheckTime = 0;
        receiver.gameObject.SetActive(true);
        StartCoroutine(handLandmarker.Resume());

        isDetecting = true;
    }

    // チェック終了
    public void EndCheck()
    {
        isDetecting = false;

        // on check over
        if (!isInitializing) OnHandDetectionOver?.Invoke(point);

        // stop checking
        Initalize();
        receiver.gameObject.SetActive(false);
        handLandmarker.Pause();
    }

    private void FixedUpdate()
    {
        if (!isDetecting) return;

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
            // Debug.Log("手が2つ無い");
            HeartTimeCount = 0;
            return;
        }

        // 手の形がハートになっているかのチェック
        // if (receiver.AreAllFingertipsHigherThanBase(receiver.result) || receiver.AreFingertipsBentInward(receiver.result) || receiver.IsThumbTipHighest(receiver.result))
        if (receiver.AreAllFingertipsHigherThanBase(receiver.result) || receiver.IsThumbTipHighest(receiver.result))
        {
            // Debug.Log("手の形がハートではない");
            HeartTimeCount = 0;
            return;
        }

        HeartTimeCount += Time.deltaTime;

        // 手がしばらく止まっている
        if (receiver.isFleezCount[0] > 3 && receiver.isFleezCount[1] > 3 && HeartTimeCount > 0.4f)
        {
            // フェーズ移行処理
            SetStartHandDistance();

            point = 0;
            NextPhase();
            HeartTimeCount = 0;
            // Debug.Log("チェックスタート");

            // on check start (show moe1 anim)
            OnHandCheckStart?.Invoke();
        }
    }

    // フェーズ2
    private void Phase2()
    {
        // 手の距離のチェック
        // if (HandDistanceCheck()) ResetDueToDistruption();

        // 手の移動の更新
        UpdateMoveDir();

        // 判定のチェック
        if (nowCheckTime >= checkTime[phase - 1])
        {
            // 一定量横に移動したかのチェック
            if (Mathf.Abs(HandMoveDirX) > 0.09f) point++;
            NextPhase();
            // Debug.Log("萌え");

            // on check proceed (show moe2 anim)
            OnHandDetectionProceed?.Invoke(phase);
        }
    }

    private void Phase3()
    {
        // 手の距離のチェック
        // if (HandDistanceCheck()) ResetDueToDistruption();

        // 手の移動の更新
        UpdateMoveDir();

        // 判定のチェック
        if (nowCheckTime >= checkTime[phase - 1])
        {
            // 一定量横に移動したかのチェック
            if (Mathf.Abs(HandMoveDirX) > 0.09f) point++;
            NextPhase();
            // Debug.Log("萌え");

            // on check proceed (show kyun anim)
            OnHandDetectionProceed?.Invoke(phase);
        }
    }

    private void Phase4()
    {
        // 手の距離のチェック
        // if (HandDistanceCheck()) EndCheck();

        // 手の移動の更新
        UpdateMoveDir();

        // 判定のチェック
        if (nowCheckTime >= checkTime[phase - 1])
        {
            // 一定量横に移動したかのチェック
            if (Mathf.Abs(HandMoveDirX) > 0.04f) point++;
            // Debug.Log("キュン");
            EndCheck();
        }
    }

    // 初期化
    private void ResetDueToDistruption()
    {
        phase = 1;
        HandMoveDirX = 0;
        HandMoveDirY = 0;
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
        phase++;
        nowCheckTime = 0;
        HandMoveDirX = 0;
        HandMoveDirY = 0;
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