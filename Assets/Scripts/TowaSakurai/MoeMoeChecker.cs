using UnityEngine;

public class MoeMoeChecker : MonoBehaviour
{
    [SerializeField] private HandDataReceiver receiver;
    [SerializeField] private int phase;

    private float prevHandMoveDirX;
    public float HandMoveDirX;
    private float startHandDistance;
    private int nextMoveDir;

    void Start()
    {
        phase = 1;
        startHandDistance = 0;
        HandMoveDirX = 0;
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

    private void Phase1()
    {
        // 手がしばらく止まっている
        if (receiver.isFleezCount[0] > 5 && receiver.isFleezCount[1] > 5)
        {
            SetStartHandDistance();

            HandMoveDirX = 0;
            nextMoveDir = 0;

            phase = 2;
            Debug.Log("萌え萌えキュンセット");
        }
    }

    private void Phase2()
    {
        if (HandDistanceCheck()) phase = 1;

        if (receiver.handMoveDir[0].x != prevHandMoveDirX)
        {
            prevHandMoveDirX = receiver.handMoveDir[0].x;
            HandMoveDirX += receiver.handMoveDir[0].x;
        }

        if (Mathf.Abs(HandMoveDirX) > 0.1f)
        {
            nextMoveDir = HandMoveDirX > 0 ? -1 : 1;
            HandMoveDirX = 0;
            phase = 3;
            Debug.Log("萌え");
        }
    }

    private void Phase3()
    {
        if (HandDistanceCheck()) phase = 1;

        if (receiver.handMoveDir[0].x != prevHandMoveDirX)
        {
            prevHandMoveDirX = receiver.handMoveDir[0].x;
            HandMoveDirX += receiver.handMoveDir[0].x;
        }

        if (HandMoveDirX * nextMoveDir > 0.1f)
        {
            nextMoveDir *= -1;
            HandMoveDirX = 0;
            phase = 4;
            Debug.Log("萌え");
        }
    }

    private void Phase4()
    {
        if (HandDistanceCheck()) phase = 1;

        if (receiver.handMoveDir[0].x != prevHandMoveDirX)
        {
            prevHandMoveDirX = receiver.handMoveDir[0].x;
            HandMoveDirX += receiver.handMoveDir[0].x;
        }

        if (HandMoveDirX * nextMoveDir > 0.05f)
        {
            nextMoveDir *= -1;
            phase = 1;
            Debug.Log("キュン");
            Debug.Log("萌え萌えキュン成功！");
        }
    }

    private void MoveStart()
    {
        phase = 1;
    }

    private void SetStartHandDistance()
    {
        startHandDistance = receiver.handsDistance;
    }

    private bool HandDistanceCheck()
    {
        return startHandDistance > receiver.handsDistance + 0.1 || startHandDistance < receiver.handsDistance - 0.1;
    }
}