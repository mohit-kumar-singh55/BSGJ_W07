using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MeshCollider))]// MeshColliderが必要
public class KetchupPainter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RenderTexture ketchupRT;
    [SerializeField] private Texture2D brushTexture;// 丸い画像を読み込む

    [Space(10)]
    [Header("Brush Settings")]
    [SerializeField] private int brushSize = 20;
    [SerializeField] private Color paintColor = Color.red;// ケチャップの赤

    [Space(10)]
    [Header("Current Gameobject Settings")]
    [SerializeField] private Color backgroundColor = new(1f, 0.8f, 0f);// 卵色

    // 前回のクリック位置を記録
    private Vector2? lastUV = null;

    //一度描き終えたかどうか覚えるフラグ
    private bool hasFinishedDrawing = false;

    private void Awake() => Validate();

    private void Start() => InitializeTexture();

    private void InitializeTexture()
    {
        // Render　Textureをアクティブに
        RenderTexture.active = ketchupRT;

        // 指定した色で塗りつぶす
        GL.Clear(true, true, backgroundColor);

        // アクティブを解除
        RenderTexture.active = null;

        //リセット時はフラグも外す
        hasFinishedDrawing = false;
    }

    private void Update()
    {
        var pointer = Pointer.current;
        if (!hasFinishedDrawing && pointer != null && pointer.press.isPressed)
        {
            if (IsPointerOverGameObject(out RaycastHit hit))
            {
                Vector2 currentUV = hit.textureCoord;

                // 前回の位置を埋める
                if (lastUV.HasValue)
                {
                    float distance = Vector2.Distance(lastUV.Value, currentUV);
                    int steps = Mathf.CeilToInt(distance * 500);

                    for (int i = 0; i <= steps; i++)
                    {
                        float t = (float)i / steps;
                        // Lerpを使って中間地点を計算して塗る
                        PaintAt(Vector2.Lerp(lastUV.Value, currentUV, t));
                    }
                }
                else PaintAt(currentUV);

                lastUV = currentUV;// 今回の位置を保存
            }
            else lastUV = null;// 離したらリセット
        }
        else
        {
            if (lastUV.HasValue)
            {
                hasFinishedDrawing = true;
                lastUV = null;
                Debug.Log("一筆書き終わり：もう描けません");
            }
        }
    }

    private bool IsPointerOverGameObject(out RaycastHit hit)
    {
        Pointer pointer = Pointer.current;
        if (pointer != null && pointer.press.isPressed)
        {
            Vector2 pointerPosition = pointer.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(pointerPosition);

            return Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject;  // このgameObjectに当たっているか確認
        }

        hit = default;
        return false;
    }

    private void PaintAt(Vector2 uv)
    {
        RenderTexture.active = ketchupRT;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, ketchupRT.width, ketchupRT.height, 0);

        // 塗る位置を計算
        float x = uv.x * ketchupRT.width;
        float y = (1 - uv.y) * ketchupRT.height;

        // 簡易的な四角形で塗る
        Rect rect = new(x - brushSize / 2, y - brushSize / 2, brushSize, brushSize);
        Texture texToDraw = brushTexture != null ? brushTexture : Texture2D.whiteTexture;

        Graphics.DrawTexture(rect, texToDraw, new Rect(0, 0, 1, 1), 0, 0, 0, 0, paintColor, null);

        GL.PopMatrix();
        RenderTexture.active = null;
    }

    private void Validate()
    {
        Assert.IsNotNull(ketchupRT, "Ketchup RenderTexture is not assigned.");
        Assert.IsNotNull(brushTexture, "Brush Texture is not assigned.");
    }
}