using Mediapipe.Tasks.Vision.HandLandmarker;
using UnityEngine;

public class HandMarker3D : MonoBehaviour
{
    // レシーバー
    [SerializeField] private HandDataReceiver receiver;

    // 手の位置に表示する球
    [SerializeField] private Transform[] handSpheres;

    // ハート表示用オブジェクト
    [SerializeField] private Transform heartObject;

    // 座標オフセット
    [SerializeField] private Vector3 posOffset;

    // 手の形がハートであるか
    [SerializeField] private bool isHeart;

    private Vector3 prevHandPos;

    private int isFleezCount;
    // カメラ
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        isFleezCount = 0;
    }

    private void Update()
    {
        if (receiver == null ||
            receiver.result.handLandmarks == null ||
            receiver.result.handLandmarks.Count == 0 ||
            isFleezCount >= 30)
        {
            DisableAll();
            if (isFleezCount >= 30 && !CheckFleez())
                isFleezCount = 0;
            return;
        }

        CheckFleez();

        // 左右の手インデックス取得
        var (left, right) = GetHandIndices();

        // 左手
        if (left != -1)
            UpdateHandSphere(0, left);
        else
            handSpheres[0].gameObject.SetActive(false);

        // 右手
        if (right != -1)
            UpdateHandSphere(1, right);
        else
            handSpheres[1].gameObject.SetActive(false);

        // ハート判定
        UpdateIsHeart();

        // ハート表示
        UpdateHeart(left, right);
    }

    private void DisableAll()
    {
        foreach (var s in handSpheres)
            s.gameObject.SetActive(false);

        if (heartObject != null)
            heartObject.gameObject.SetActive(false);
    }

    private (int left, int right) GetHandIndices()
    {
        int leftIndex = -1;
        int rightIndex = -1;

        for (int i = 0; i < receiver.result.handedness.Count; i++)
        {
            string handName = receiver.result.handedness[i]
                .categories[0].categoryName;

            if (handName == "Left")
                leftIndex = i;
            else if (handName == "Right")
                rightIndex = i;
        }

        if (IsCameraRotated180())
        {
            int tmp = leftIndex;
            leftIndex = rightIndex;
            rightIndex = tmp;
        }

        return (leftIndex, rightIndex);
    }


    bool IsCameraRotated180()
    {
        float y = cam.transform.eulerAngles.y;

        return Mathf.Abs(Mathf.DeltaAngle(y, 180f)) < 20f;
    }


    private void UpdateHandSphere(int sphereIndex, int handIndex)
    {
        handSpheres[sphereIndex].gameObject.SetActive(true);

        Vector3 hand = GetHandCenter(handIndex);

        Vector3 screenPos = new Vector3(
            hand.x * Screen.width,
            (1f - hand.y) * Screen.height,
            1.0f
        );

        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);

        handSpheres[sphereIndex].position = worldPos + posOffset;
        handSpheres[sphereIndex].localScale = Vector3.one * 0.1f;
    }

    /// <summary>
    /// 小指付け根・人差し指付け根・手首の3点から
    /// 手のひらの中心を求める
    /// </summary>
    /// <param name="handIndex">手のインデックス</param>
    /// <returns></returns>
    public Vector3 GetHandCenter(int handIndex)
    {
        if (receiver.result.handLandmarks.Count <= handIndex)
            return Vector3.zero;

        var lm = receiver.result.handLandmarks[handIndex].landmarks;

        Vector3 wrist = new Vector3(lm[0].x, lm[0].y, lm[0].z);
        Vector3 indexBase = new Vector3(lm[5].x, lm[5].y, lm[5].z);
        Vector3 pinkyBase = new Vector3(lm[17].x, lm[17].y, lm[17].z);

        return (wrist + indexBase + pinkyBase) / 3f;
    }

    private void UpdateHeart(int leftIndex, int rightIndex)
    {
        if (!isHeart || heartObject == null)
        {
            heartObject?.gameObject.SetActive(false);
            return;
        }

        if (leftIndex == -1 || rightIndex == -1)
        {
            heartObject.gameObject.SetActive(false);
            return;
        }

        Vector3 left = GetHandCenter(leftIndex);
        Vector3 right = GetHandCenter(rightIndex);

        Vector3 leftScreen = new Vector3(left.x * Screen.width, (1f - left.y) * Screen.height, 1f);
        Vector3 rightScreen = new Vector3(right.x * Screen.width, (1f - right.y) * Screen.height, 1f);

        Vector3 leftWorld = cam.ScreenToWorldPoint(leftScreen);
        Vector3 rightWorld = cam.ScreenToWorldPoint(rightScreen);

        Vector3 center = (leftWorld + rightWorld) * 0.5f;

        heartObject.gameObject.SetActive(true);
        heartObject.position = center + posOffset;
    }

    /// <summary>
    /// IsHeartの更新
    /// </summary>
    private void UpdateIsHeart()
    {
        isHeart = !(receiver.AreAllFingertipsHigherThanBase(receiver.result)
                 || receiver.AreFingertipsBentInward(receiver.result)
                 || receiver.IsThumbTipHighest(receiver.result));
    }

    private bool CheckFleez()
    {
        if (receiver.handPos[0] == prevHandPos)
        {
            isFleezCount++;
            return true;
        }
        else
        {
            isFleezCount = 0;
        }

        prevHandPos = receiver.handPos[0];
        return false;
    }
}
