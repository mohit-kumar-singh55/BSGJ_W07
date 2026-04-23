using UnityEngine;

public class Soundtest : MonoBehaviour
{   //SE用テスト
    public AudioManager audioManager;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            audioManager.PlaySFX(audioManager.SE_Start01);

        }
    }
}
