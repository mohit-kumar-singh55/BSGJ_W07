using UnityEngine;
using UnityEngine.InputSystem;

public class KetchupPainter : MonoBehaviour
{
    public RenderTexture ketchupRT;
    public Texture2D brushTexture;//丸い画像を読み込む
    public Color backgroundColor = new Color(1f, 0.8f, 0f);//卵色
    public Color paintColor = Color.red;//ケチャップの赤
    public int brushSize = 20;

    //前回のクリック位置を記録
    private Vector2? lastUV = null;
    void Start()
    {
        InitializeTexture();
    }

    void InitializeTexture()
    {
        //Render　Textureをアクティブに
        RenderTexture.active = ketchupRT;

        //指定した色で塗りつぶす
        GL.Clear(true, true, backgroundColor);

        //アクティブを解除
        RenderTexture.active = null;
    }
    void Update()
    {
        var pointer = Pointer.current;
        if (pointer != null && pointer.press.isPressed)
        {
            Vector2 pointerPosition = pointer.position.ReadValue();

            Ray ray = Camera.main.ScreenPointToRay(pointerPosition);
            RaycastHit hit;

            //MeshColliderがある場合教える
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    Vector2 currentUV = hit.textureCoord;

                    //前回の位置を埋める
                    if (lastUV.HasValue)
                    {
                        float distance = Vector2.Distance(lastUV.Value, currentUV);
                        int steps = Mathf.CeilToInt(distance * 500);

                        for (int i = 0; i <= steps; i++)
                        {
                            float t = (float)i / steps;
                            //Lerpを使って中間地点を計算して塗る
                            PaintAt(Vector2.Lerp(lastUV.Value, currentUV, t));
                        }

                    }
                    else
                    {
                        PaintAt(currentUV);
                    }

                    lastUV = currentUV;//今回の位置を保存
                }
            }

        }
        else
        {
            lastUV = null;//話したらリセット
        }
    }

    void PaintAt(Vector2 uv)
    {
        RenderTexture.active = ketchupRT;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, ketchupRT.width, ketchupRT.height, 0);

        //塗る位置を計算
        float x = uv.x * ketchupRT.width;
        float y = (1 - uv.y) * ketchupRT.height;

        //簡易的な四角形で塗る
        Rect rect = new Rect(x - brushSize / 2, y - brushSize / 2, brushSize, brushSize);
        Texture texToDraw = brushTexture != null ? brushTexture : Texture2D.whiteTexture;

        Graphics.DrawTexture(rect, texToDraw, new Rect(0, 0, 1, 1), 0, 0, 0, 0, paintColor, null);

        GL.PopMatrix();
        RenderTexture.active = null;
    }

    void Osable()
    {
        if (ketchupRT != null) InitializeTexture();
    }


}
