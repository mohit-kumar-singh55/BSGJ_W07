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

    // カメラ
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (receiver == null ||
            receiver.result.handLandmarks == null ||
            receiver.result.handLandmarks.Count == 0)
            return;



        // 手の球の更新
        for (int i = 0; i < handSpheres.Length; i++)
        {
            if (i >= receiver.result.handLandmarks.Count)
            {
                    handSpheres[i].gameObject.SetActive(false);
                continue;
            }

            handSpheres[i].gameObject.SetActive(true);

            Vector3 hand = GetHandCenter(i);

            Vector3 screenPos = new Vector3(
                hand.x * Screen.width,
                (1f - hand.y) * Screen.height,
                1.0f
            );

            Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);

            handSpheres[i].position = worldPos + posOffset;

            float scale = 0.1f;
            handSpheres[i].localScale = new Vector3(scale, scale, scale);
        }

        // 手がハートであるかのチェック
        UpdateIsHeart();

        // ハート表示処理
        UpdateHeart();
    }



    /// <summary>
    /// 小指付け根・人差し指付け根・手首の3点から
    /// 手のひらの中心を求める
    /// </summary>
    /// <param name="handIndex">手のインデックス</param>
    /// <returns></returns>
    public Vector3 GetHandCenter(int handIndex)
    {
        if ( receiver.result.handLandmarks.Count <= handIndex)
            return Vector3.zero;

        var lm = receiver.result.handLandmarks[handIndex].landmarks;

        Vector3 wrist = new Vector3(lm[0].x, lm[0].y, lm[0].z);
        Vector3 indexBase = new Vector3(lm[5].x, lm[5].y, lm[5].z);
        Vector3 pinkyBase = new Vector3(lm[17].x, lm[17].y, lm[17].z);

        // 重心（3点の平均）
        Vector3 center = (wrist + indexBase + pinkyBase) / 3f;

        return center;
    }

    private void UpdateHeart()
    {
        if (!isHeart || heartObject == null)
        {
            if (heartObject != null)
                heartObject.gameObject.SetActive(false);
            return;
        }

        // 両手が存在しない場合は非表示
        if (receiver.result.handLandmarks.Count < 2)
        {
            heartObject.gameObject.SetActive(false);
            return;
        }

        // 両手の中心を計算
        Vector3 left = GetHandCenter(0);
        Vector3 right = GetHandCenter(1);

        // スクリーン座標へ
        Vector3 leftScreen = new Vector3(left.x * Screen.width, (1f - left.y) * Screen.height, 1f);
        Vector3 rightScreen = new Vector3(right.x * Screen.width, (1f - right.y) * Screen.height, 1f);

        // ワールド座標へ
        Vector3 leftWorld = cam.ScreenToWorldPoint(leftScreen);
        Vector3 rightWorld = cam.ScreenToWorldPoint(rightScreen);

        // 中心位置
        Vector3 center = (leftWorld + rightWorld) * 0.5f;

        // ハートを表示
        heartObject.gameObject.SetActive(true);
        heartObject.position = center + posOffset;
    }
    /// <summary>
    /// IsHeartの更新
    /// </summary>
    private void UpdateIsHeart()
    {
        isHeart = !(receiver.AreAllFingertipsHigherThanBase(receiver.result) || receiver.AreFingertipsBentInward(receiver.result) || receiver.IsThumbTipHighest(receiver.result));
    }
}