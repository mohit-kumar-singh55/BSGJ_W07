using System.Collections.Generic;
using UnityEngine;

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

    public void PlayBGM(BGM bgmType)
    {
        if (!_bgmDictionary.TryGetValue(bgmType, out AudioClip clip)) return;
        if (_bgmSource.clip == clip && _bgmSource.isPlaying) return;

        _bgmSource.clip = clip;
        _bgmSource.Play();
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
        if (_sfxSource.isPlaying) return;
        _sfxSource.PlayOneShot(clip);
    }
}