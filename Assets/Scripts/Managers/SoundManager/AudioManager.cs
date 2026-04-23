using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class AudioManager : Singleton<AudioManager>
{

    [Header("Audio Source")]
    [SerializeField] AudioSource Master;

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clip")]
    //BGM
    public AudioClip background;
    //SE
    public AudioClip SE_Burst01;
    public AudioClip SE_GuestComing02;
    public AudioClip SE_Ketchup08;
    public AudioClip SE_MoeKyun02;
    public AudioClip SE_Option02;
    public AudioClip SE_Satisfied1;
    public AudioClip SE_Satisfied2;
    public AudioClip SE_Satisfied3;
    public AudioClip SE_Score01;
    public AudioClip SE_Select02;
    public AudioClip SE_Start01;





    public void PlayBGM(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    // Update is called once per frame
    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
