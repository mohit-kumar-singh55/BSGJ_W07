using UnityEngine;

public class SystemSetup : MonoBehaviour
{
    private void Start()
    {
        // setup
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
        Screen.SetResolution(2560, 1440, true); // ! maybe better to comment it when in release for the jam to increase quality
    }
}