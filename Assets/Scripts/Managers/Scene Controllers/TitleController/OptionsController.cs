using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

/// <summary>
/// オプション画面のコントローラー
/// </summary>
[RequireComponent(typeof(TitleController))]
public class OptionsController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("Element Names")]
    [SerializeField] private string _backButtonString = "BackButton";
    [SerializeField] private string _micDropdownString = "Mic";
    [SerializeField] private string _langDropdownString = "Language";
    [SerializeField] private string _soundSliderString = "Sound";

    private GameManager _gameManager;
    private DropdownField _micDropdown;
    private DropdownField _langDropdown;
    private Slider _soundSlider;
    private TitleController _titleController;

    private const string MASTER_VOLUME_MIXER_PARAM = "MasterVolume";

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _titleController = GetComponent<TitleController>();

        Button backButton = _titleController.OptionsUI.Q<Button>(_backButtonString);
        _micDropdown = _titleController.OptionsUI.Q<DropdownField>(_micDropdownString);
        _langDropdown = _titleController.OptionsUI.Q<DropdownField>(_langDropdownString);
        _soundSlider = _titleController.OptionsUI.Q<Slider>(_soundSliderString);

        // go back to title
        backButton.clicked += _titleController.ShowMenuPanel;

        // マイクが接続されていない場合は終了する
        if (Microphone.devices.Length <= 0) return;

        // mic selection
        _micDropdown.value = Microphone.devices[GetMicIndex()];   // 現在使用中のマイクを初期値として設定する
        _micDropdown.RegisterValueChangedCallback(SetMicToUse);

        // populate mic list
        _micDropdown.choices = new List<string>(Microphone.devices);

        // language selection
        _langDropdown.value = GetCurrentLanguage();
        _langDropdown.RegisterValueChangedCallback(SetLanguage);

        // sound slider
        _soundSlider.value = GetCurrentMasterVolume();
        _soundSlider.RegisterValueChangedCallback(SetMasterVolume);
    }

    private void SetMicToUse(ChangeEvent<string> evt)
    {
        PlayerPrefs.SetInt(PLAYER_PREFS.CURRENT_MIC_IN_USE, _micDropdown.index);
    }

    private int GetMicIndex()
    {
        int micIndex = PlayerPrefs.GetInt(PLAYER_PREFS.CURRENT_MIC_IN_USE, 0);
        micIndex = Mathf.Clamp(micIndex, 0, Microphone.devices.Length - 1);    // エラーを防ぐためにClampする

        // r今後エラーが発生しないよう、Clamp後の値をPlayerPrefsに保存する
        PlayerPrefs.SetInt(PLAYER_PREFS.CURRENT_MIC_IN_USE, micIndex);

        return micIndex;
    }

    private string GetCurrentLanguage()
    {
        LANG currentLang = (LANG)PlayerPrefs.GetInt(PLAYER_PREFS.CURRENT_LANGUAGE, (int)LANG.JAPANESE);   // 現在の言語を取得する（デフォルトは日本語）
        _gameManager.CurrentLanguage = currentLang;   // GameManagerに現在の言語を設定する
        return currentLang.ToString();
    }

    private void SetLanguage(ChangeEvent<string> evt)
    {
        if (evt.newValue == LANG.ENGLISH.ToString())
        {
            _gameManager.CurrentLanguage = LANG.ENGLISH;
            PlayerPrefs.SetInt(PLAYER_PREFS.CURRENT_LANGUAGE, (int)LANG.ENGLISH);
        }
        else if (evt.newValue == LANG.JAPANESE.ToString())
        {
            _gameManager.CurrentLanguage = LANG.JAPANESE;
            PlayerPrefs.SetInt(PLAYER_PREFS.CURRENT_LANGUAGE, (int)LANG.JAPANESE);
        }
    }

    private float GetCurrentMasterVolume()
    {
        float curVol = PlayerPrefs.GetFloat(PLAYER_PREFS.MASTER_VOLUME, 0f);
        _audioMixer.SetFloat(MASTER_VOLUME_MIXER_PARAM, curVol);
        return curVol;
    }

    private void SetMasterVolume(ChangeEvent<float> evt)
    {
        _audioMixer.SetFloat(MASTER_VOLUME_MIXER_PARAM, evt.newValue);
        PlayerPrefs.SetFloat(PLAYER_PREFS.MASTER_VOLUME, evt.newValue);
    }
}