using UnityEngine;

[CreateAssetMenu(fileName = "New Audio SO", menuName = "AudioSO", order = 0)]
public class AudioSO : ScriptableObject
{
    [SerializeField] private AudioType _audioType = AudioType.SFX;
    [SerializeField, NaughtyAttributes.ShowIf("AudioType", AudioType.SFX)] private SFX _sfxType = SFX.Ketchup;
    [SerializeField, NaughtyAttributes.ShowIf("AudioType", AudioType.BGM)] private BGM _bgmType = BGM.FeverMode;
    [SerializeField] private AudioClip _audioClip;

    public AudioType AudioType => _audioType;
    public SFX SFXType => _sfxType;
    public BGM BGMType => _bgmType;
    public AudioClip Clip => _audioClip;
}