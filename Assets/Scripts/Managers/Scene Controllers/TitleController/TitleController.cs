using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// タイトル画面のUIを管理するコントローラークラス
/// </summary>
[RequireComponent(typeof(UIDocument))]
public class TitleController : MonoBehaviour
{
    private UIDocument _uiPanel;
    private VisualElement _menuUI;
    private VisualElement _optionsUI;
    private VisualElement _trailerVideoUI;

    public VisualElement MenuUI => _menuUI;
    public VisualElement OptionsUI => _optionsUI;

    private const string MENU_UI_NAME = "Menu";
    private const string OPTIONS_UI_NAME = "Options";
    private const string TRAILER_VIDEO_UI_NAME = "TrailerVideoContainer";

    private void Awake()
    {
        _uiPanel = GetComponent<UIDocument>();

        VisualElement root = _uiPanel.rootVisualElement;

        // get all UI
        _menuUI = root.Q<VisualElement>(MENU_UI_NAME);
        _optionsUI = root.Q<VisualElement>(OPTIONS_UI_NAME);
        _trailerVideoUI = root.Q<VisualElement>(TRAILER_VIDEO_UI_NAME);

        // 初期状態ではメニューUIのみ表示する
        ShowMenuPanel();
    }

    public void ShowOptionsPanel()
    {
        _optionsUI.style.display = DisplayStyle.Flex;
        _menuUI.style.display = DisplayStyle.None;
        _trailerVideoUI.style.display = DisplayStyle.None;
    }

    public void ShowMenuPanel()
    {
        _menuUI.style.display = DisplayStyle.Flex;
        _optionsUI.style.display = DisplayStyle.None;
        _trailerVideoUI.style.display = DisplayStyle.None;
    }

    public void ShowTrailerVideoUIPanel()
    {
        _trailerVideoUI.style.display = DisplayStyle.Flex;
        _menuUI.style.display = DisplayStyle.None;
        _optionsUI.style.display = DisplayStyle.None;
    }
}