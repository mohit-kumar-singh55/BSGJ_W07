using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class OptionsController : TitleController
{
    [Header("Element Names")]
    [SerializeField] private string _backButtonString = "BackButton";
    [SerializeField] private string _micDropdownString = "Mic";

    private DropdownField _micDropdown;

    protected override void Start()
    {
        base.Start();

        Button backButton = OptionsUI.Q<Button>(_backButtonString);
        _micDropdown = OptionsUI.Q<DropdownField>(_micDropdownString);

        // go back to title
        backButton.clicked += ShowMenuPanel;

        // return if there are no microphones connected
        if (Microphone.devices.Length <= 0) return;

        // mic selection
        _micDropdown.value = Microphone.devices[PlayerPrefs.GetInt(PLAYER_PREFS.CURRENT_MIC_IN_USE, 0)];
        _micDropdown.RegisterValueChangedCallback(SetMicToUse);

        // populate mic list
        _micDropdown.choices = new List<string>(Microphone.devices);
    }

    private void SetMicToUse(ChangeEvent<string> evt)
    {
        PlayerPrefs.SetInt(PLAYER_PREFS.CURRENT_MIC_IN_USE, _micDropdown.index);
    }
}