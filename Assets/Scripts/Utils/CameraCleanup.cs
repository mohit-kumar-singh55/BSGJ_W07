#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class CameraCleanup
{
#if UNITY_EDITOR
    static CameraCleanup()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            var cams = Resources.FindObjectsOfTypeAll<WebCamTexture>();
            foreach (var cam in cams)
            {
                if (cam.isPlaying)
                    cam.Stop();
            }

            Debug.Log("Stopped all webcams (Editor cleanup)");
        }
    }
#endif
}