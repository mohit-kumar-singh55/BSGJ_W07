using UnityEngine;

public class HandMarker3D : MonoBehaviour
{
    // レシーバー
    [SerializeField] private HandDataReceiver receiver;

    // 手の位置に表示する球
    [SerializeField] private Transform[] handSpheres;

    // 座標オフセット
    [SerializeField] private Vector3 posOffset;

    private Vector3 prevHandPos;

    private int isFreezeCount;
    // カメラ
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        isFreezeCount = 0;
    }

    private void Update()
    {
        if (receiver == null ||
            receiver.result.handLandmarks == null ||
            receiver.result.handLandmarks.Count == 0 ||
            isFreezeCount >= 30)
        {
            DisableAll();
            if (isFreezeCount >= 30 && !CheckFreeze())
                isFreezeCount = 0;
            return;
        }

        CheckFreeze();

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
    }

    private void DisableAll()
    {
        foreach (var s in handSpheres)
            s.gameObject.SetActive(false);
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

        return (leftIndex, rightIndex);
    }

    private void UpdateHandSphere(int sphereIndex, int handIndex)
    {
        handSpheres[sphereIndex].gameObject.SetActive(true);

        Vector3 hand = GetHandCenter(handIndex);

        Vector3 screenPos = new(
            hand.x * Screen.width,
            (1f - hand.y) * Screen.height,
            1.0f
        );

        Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);

        handSpheres[sphereIndex].position = Vector3.Lerp(handSpheres[sphereIndex].position, worldPos + posOffset, 10 * Time.deltaTime);
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

        Vector3 wrist = new(lm[0].x, lm[0].y, lm[0].z);
        Vector3 indexBase = new(lm[5].x, lm[5].y, lm[5].z);
        Vector3 pinkyBase = new(lm[17].x, lm[17].y, lm[17].z);

        return (wrist + indexBase + pinkyBase) / 3f;
    }

    private bool CheckFreeze()
    {
        if (receiver.handPos[0] == prevHandPos)
        {
            isFreezeCount++;
            return true;
        }
        else
            isFreezeCount = 0;

        prevHandPos = receiver.handPos[0];
        return false;
    }
}