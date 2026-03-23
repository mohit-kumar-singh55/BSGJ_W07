using UnityEngine;
using UnityEngine.UIElements;

public class TitleSceneController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIDocument _titleUIPanel;
    // [SerializeField] private GameObject _settingPanel;

    [Header("Element Names")]
    [SerializeField] private string _startButton = "StartButton";
    // [SerializeField] private string _settingButton = "SettingButton";
    [SerializeField] private string _playerNameInput = "NameInput";

    void Start()
    {
        VisualElement root = _titleUIPanel.rootVisualElement;

        Button startButton = root.Q<Button>(_startButton);
        // Button settingButton = root.Q<Button>(_settingButton);
        TextField playerName = root.Q<TextField>(_playerNameInput);

        startButton.clicked += () =>
        {
            // get name from input before going to next scene
            if (string.IsNullOrEmpty(playerName.value))
            {
                playerName.Focus();
                return;
            }

            PlayerDataManager.Instance.SetPlayerName(playerName.value);
            GameManager.Instance.GoNextScene();
        };
        // settingButton.clicked += ShowSettingPanel;
    }

    // private void ShowSettingPanel() => _settingPanel.SetActive(true);
}