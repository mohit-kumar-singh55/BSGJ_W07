using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MenuController : TitleController
{
    [Header("Element Names")]
    [SerializeField] private string _startButton = "StartButton";
    [SerializeField] private string _optionsButton = "OptionsButton";
    [SerializeField] private string _playerNameInput = "NameInput";

    protected override void Start()
    {
        base.Start();

        Button startButton = MenuUI.Q<Button>(_startButton);
        Button optionsButton = MenuUI.Q<Button>(_optionsButton);
        TextField playerName = MenuUI.Q<TextField>(_playerNameInput);

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

        optionsButton.clicked += ShowOptionsPanel;
    }
}