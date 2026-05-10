using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    [Space(10)]
    [Header("Audios")]
    [SerializeField] private AudioSO[] _allAudios;

    private Dictionary<SFX, AudioClip> _sfxDictionary = new();
    private Dictionary<BGM, AudioClip> _bgmDictionary = new();

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        // initialize dictionaries
        foreach (AudioSO audio in _allAudios)
        {
            if (audio.AudioType == AudioType.SFX)
                _sfxDictionary[audio.SFXType] = audio.Clip;
            else
                _bgmDictionary[audio.BGMType] = audio.Clip;
        }
    }

    private void Start()
    {
        // playing title on start
        PlayBGM(BGM.Title);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == SCENES.TITLE)
            PlayBGM(BGM.Title);
        else
            PlayBGM(BGM.Mainbgm);
    }

    public void PlayBGM(BGM bgmType, float fadeTime = 1f)
    {
        if (!_bgmDictionary.TryGetValue(bgmType, out AudioClip clip)) return;
        if (_bgmSource.clip == clip) return;

        StartCoroutine(FadeOut(_bgmSource, fadeTime, () =>
       {
           _bgmSource.clip = clip;
           _bgmSource.Play();
       }));
    }

    public void PlaySFX(SFX sfxType)
    {
        if (!_sfxDictionary.TryGetValue(sfxType, out AudioClip clip)) return;
        if (_sfxSource.isPlaying) return;
        _sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioSource audioSource, SFX sfxType)
    {
        if (!_sfxDictionary.TryGetValue(sfxType, out AudioClip clip)) return;
        if (audioSource.isPlaying) return;
        audioSource.PlayOneShot(clip);
    }

    private IEnumerator FadeOut(AudioSource audioSource, float fadeTime, Action callback)
    {
        float startVolume = audioSource.volume;
        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.unscaledDeltaTime / fadeTime;
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = startVolume;
        callback?.Invoke();
    }
}