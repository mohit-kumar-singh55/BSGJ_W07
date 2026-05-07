using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class OptionsController : TitleController
{
    [Header("Element Names")]
    [SerializeField] private string _backButtonString = "BackButton";
    [SerializeField] private string _micDropdownString = "Mic";
    [SerializeField] private string _langDropdownString = "Language";

    private DropdownField _micDropdown;
    private DropdownField _langDropdown;

    protected override void Start()
    {
        base.Start();

        Button backButton = OptionsUI.Q<Button>(_backButtonString);
        _micDropdown = OptionsUI.Q<DropdownField>(_micDropdownString);
        _langDropdown = OptionsUI.Q<DropdownField>(_langDropdownString);

        // go back to title
        backButton.clicked += ShowMenuPanel;

        // return if there are no microphones connected
        if (Microphone.devices.Length <= 0) return;

        // mic selection
        _micDropdown.value = Microphone.devices[GetMicIndex()];   // set default value to current mic in use
        _micDropdown.RegisterValueChangedCallback(SetMicToUse);

        // populate mic list
        _micDropdown.choices = new List<string>(Microphone.devices);

        // language selection
        _langDropdown.value = GetCurrentLanguage();
        _langDropdown.RegisterValueChangedCallback(SetLanguage);
    }

    private void SetMicToUse(ChangeEvent<string> evt)
    {
        PlayerPrefs.SetInt(PLAYER_PREFS.CURRENT_MIC_IN_USE, _micDropdown.index);
    }

    private int GetMicIndex()
    {
        int micIndex = PlayerPrefs.GetInt(PLAYER_PREFS.CURRENT_MIC_IN_USE, 0);
        micIndex = Mathf.Clamp(micIndex, 0, Microphone.devices.Length - 1);    // clamp to avoid error

        // resave clamped value to player prefs to avoid error in future
        PlayerPrefs.SetInt(PLAYER_PREFS.CURRENT_MIC_IN_USE, micIndex);

        return micIndex;
    }

    private string GetCurrentLanguage()
    {
        LANG currentLang = (LANG)PlayerPrefs.GetInt(PLAYER_PREFS.CURRENT_LANGUAGE, (int)LANG.JAPANESE);   // get current language, default to japanese
        return currentLang.ToString();
    }

    private void SetLanguage(ChangeEvent<string> evt)
    {
        if (evt.newValue == LANG.ENGLISH.ToString())
        {
            GameManager.Instance.CurrentLanguage = LANG.ENGLISH;
            PlayerPrefs.SetInt(PLAYER_PREFS.CURRENT_LANGUAGE, (int)LANG.ENGLISH);
        }
        else if (evt.newValue == LANG.JAPANESE.ToString())
        {
            GameManager.Instance.CurrentLanguage = LANG.JAPANESE;
            PlayerPrefs.SetInt(PLAYER_PREFS.CURRENT_LANGUAGE, (int)LANG.JAPANESE);
        }
    }
}