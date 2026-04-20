using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MeshCollider))]
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
    [SerializeField] private Texture2D sampleGuideTexture;
    [SerializeField] private RenderTexture sampleRT;
    [SerializeField] private Color sampleColor = new(1, 1, 1, 0.5f);

    [Space(10)]
    [Header("Draw Settings")]
    [SerializeField] private int maxDrawCount = 1;

    [Space(10)]
    [Header("Score Settings")]
    [Tooltip("Score is calculated out of 100% and then multiplied by this value")]
    [SerializeField] private float scoreMultiplier = 1.0f;

    // 前回のクリック位置を記録
    private Vector2? lastUV = null;

    //一度描き終えたかどうか覚えるフラグ
    private bool hasFinishedDrawing = false;
    private bool wasPressing = false;
    private bool isDrawing = false;
    private int currentDrawCount = 0;
    private bool allowedToDraw = false;
    private UIManager _uiManager;

    public static event System.Action<int> OnFinishedDrawing = delegate { };

    void OnEnable()
    {
        PlayerPainting.OnPlayerEnterPainting += AllowDrawing;
        PlayerPainting.OnPlayerExitPainting += DisallowDrawing;
    }

    void OnDisable()
    {
        PlayerPainting.OnPlayerEnterPainting -= AllowDrawing;
        PlayerPainting.OnPlayerExitPainting -= DisallowDrawing;
    }

    private void Awake() => Validate();

    private void Start()
    {
        currentDrawCount = 0;
        InitializeTexture();
        InitializeSampleFromImage();
        _uiManager = UIManager.Instance;
    }

    private void InitializeTexture()
    {
        // Render　Textureをアクティブに
        RenderTexture.active = ketchupRT;

        // 指定した色で塗りつぶす
        GL.Clear(true, true, Color.clear);

        // アクティブを解除
        RenderTexture.active = null;

        //リセット時はフラグも外す
        hasFinishedDrawing = false;
    }

    private void InitializeSampleFromImage()
    {
        RenderTexture.active = sampleRT;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, sampleRT.width, sampleRT.height, 0);

        Rect rect = new(0, 0, sampleRT.width, sampleRT.height);

        Graphics.DrawTexture(rect, sampleGuideTexture, new Rect(0, 0, 1, 1), 0, 0, 0, 0, sampleColor);
        GL.PopMatrix();
        RenderTexture.active = null;
    }

    private void Update()
    {
        if (!allowedToDraw) return;

        var pointer = Pointer.current;
        bool pressing = pointer != null && pointer.press.isPressed;

        if (!hasFinishedDrawing && pressing)
        {
            if (IsPointerOverGameObject(out RaycastHit hit))
            {
                isDrawing = true;
                Vector2 currentUV = hit.textureCoord;

                // 前回の位置を埋める
                if (lastUV.HasValue)
                {
                    float distance = Vector2.Distance(lastUV.Value, currentUV);
                    int steps = Mathf.CeilToInt(distance * 50);

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
        }

        if (isDrawing && wasPressing && !pressing)
        {
            currentDrawCount++;
            if (currentDrawCount >= maxDrawCount)
            {
                hasFinishedDrawing = true;

                float score = CalculateScorePercent() * scoreMultiplier;
                // Debug.Log("完成度:" + score.ToString("F1") + "%");
                // Debug.Log("もう描けません");

                // スコアを送る
                OnFinishedDrawing?.Invoke(Mathf.RoundToInt(score));
            }
            // else Debug.Log("書いた回数:" + currentDrawCount + "/" + maxDrawCount);

            isDrawing = false;
            lastUV = null;
        }

        wasPressing = pressing;
    }

    void FixedUpdate()
    {
        // update ui
        _uiManager.UpdateRemainingStokes(maxDrawCount - currentDrawCount);
    }

    private void AllowDrawing() => allowedToDraw = true;

    private void DisallowDrawing() => allowedToDraw = false;

    private bool IsPointerOverGameObject(out RaycastHit hit)
    {
        Pointer pointer = Pointer.current;
        if (pointer != null && pointer.press.isPressed)
        {
            Vector2 pointerPosition = pointer.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(pointerPosition);

            LayerMask mask = LayerMask.GetMask("PaintTarget");
            return Physics.Raycast(ray, out hit, 100f, mask) && hit.collider.gameObject == gameObject;  // このgameObjectに当たっているか確認
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

    private Texture2D ReadTexture(RenderTexture rt)
    {
        RenderTexture current = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D tex = new(rt.width, rt.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        RenderTexture.active = current;

        return tex;
    }

    private float CalculateScorePercent()
    {
        Texture2D sample = ReadTexture(sampleRT);
        Texture2D ketchup = ReadTexture(ketchupRT);

        Color[] samplePixels = sample.GetPixels();
        Color[] ketchupPixels = ketchup.GetPixels();

        int totalGuidePixels = 0;
        int correctPaintedPixels = 0;
        int overflowPixels = 0;

        for (int i = 0; i < samplePixels.Length; i++)
        {
            bool isGuide = samplePixels[i].a > 0.1f;
            bool isPainted = ketchupPixels[i].a > 0.1f;

            if (isGuide)
            {
                totalGuidePixels++;
                if (isPainted) correctPaintedPixels++;
            }
            else if (isPainted) overflowPixels++;
        }

        //なぞり具合
        float coverage = (float)correctPaintedPixels / totalGuidePixels;
        //清潔度
        float allowedOverflow = totalGuidePixels * 5.0f;

        //許容範囲の超え具合(清潔度を下げる)
        float excessOverflow = Mathf.Max(0, overflowPixels - allowedOverflow);

        //ガイド面積の５倍以上塗ったら清潔度は０
        float cleanliness = Mathf.Max(0, 1.0f - (excessOverflow / (totalGuidePixels * 5.0f)));

        //最終スコア
        float finalScore = coverage * cleanliness * 100f;

        //１００％をでやすくする調整
        if (coverage > 0.9f && cleanliness > 0.9f) return 100f;
        return Mathf.Clamp(finalScore, 0f, 100f);
    }

    private void Validate()
    {
        Assert.IsNotNull(ketchupRT, "Ketchup RenderTexture is not assigned.");
        Assert.IsNotNull(brushTexture, "Brush Texture is not assigned.");
        Assert.IsNotNull(sampleGuideTexture, "Sample Guide Texture is not assigned.");
        Assert.IsNotNull(sampleRT, "Sample Render Texture is not assigned.");
    }
}