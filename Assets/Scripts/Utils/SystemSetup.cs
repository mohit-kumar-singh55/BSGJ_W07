using UnityEngine;

public class SystemSetup : MonoBehaviour
{
    private void Start()
    {
        // setup
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
        Screen.SetResolution(2560, 1440, true); // ! ジャム版のリリース時は品質向上のため、無効化した方がよいかもしれない
    }
}