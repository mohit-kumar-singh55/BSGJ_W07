using System;
using UnityEngine;
using UnityEngine.Rendering;

public class TraceScore : MonoBehaviour
{
    public RenderTexture sampleRT;
    public RenderTexture ketchupRT;

    Texture2D sampleTex;
    Texture2D ketchupTex;

    int overPixels = 0;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Check();
        }
    }

    public float CalculateScore()
    {
        sampleTex = Convert(sampleRT);
        ketchupTex = Convert(ketchupRT);

        int guidePixels = 0;
        int hitPixels = 0;

        for (int x = 0; x < sampleTex.width; x++)
        {
            for (int y = 0; y < sampleTex.height; y++)
            {
                Color guide = sampleTex.GetPixel(x, y);
                Color paint = ketchupTex.GetPixel(x, y);

                //点線判定
                if (guide.a > 0.5f)
                {
                    guidePixels++;

                    //ケチャップが乗っている
                    if (IsPaintNear(x, y))
                    {
                        hitPixels++;
                    }

                }
                if (guide.a <= 0.5f && paint.r > 0.8f && paint.g < 0.3f)
                {
                    overPixels++;
                }
            }
        }
        float score = (float)hitPixels / guidePixels * 100f;
        return score;
    }
    Texture2D Convert(RenderTexture rt)
    {
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        return tex;

    }

    public void Check()
    {
        float result = CalculateScore();
        Debug.Log("スコア:" + result + "%");
    }

    bool IsPaintNear(int px, int py)
    {
        int range = 7;//許容範囲
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                int nx = px + x;
                int ny = py + y;
                if (nx < 0 || ny < 0 || nx >= ketchupTex.width || ny >= ketchupTex.height)
                    continue;
                Color paint = ketchupTex.GetPixel(nx, ny);
                if (paint.r > 0.8f && paint.g < 0.3f)
                {
                    return true;
                }
            }


        }
        return false;
    }


}
