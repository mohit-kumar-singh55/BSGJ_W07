using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public abstract class TitleController : MonoBehaviour
{
    private UIDocument _uiPanel;
    private VisualElement _menuUI;
    private VisualElement _optionsUI;
    private VisualElement _trailerVideoUI;

    protected VisualElement MenuUI => _menuUI;
    protected VisualElement OptionsUI => _optionsUI;

    private const string MENU_UI_NAME = "Menu";
    private const string OPTIONS_UI_NAME = "Options";
    private const string TRAILER_VIDEO_UI_NAME = "TrailerVideoContainer";

    protected virtual void Awake()
    {
        _uiPanel = GetComponent<UIDocument>();
    }

    protected virtual void Start()
    {
        VisualElement root = _uiPanel.rootVisualElement;

        // get all UI
        _menuUI = root.Q<VisualElement>(MENU_UI_NAME);
        _optionsUI = root.Q<VisualElement>(OPTIONS_UI_NAME);
        _trailerVideoUI = root.Q<VisualElement>(TRAILER_VIDEO_UI_NAME);

        // initially show menu only
        ShowMenuPanel();
    }

    protected void ShowOptionsPanel()
    {
        _optionsUI.style.display = DisplayStyle.Flex;
        _menuUI.style.display = DisplayStyle.None;
        _trailerVideoUI.style.display = DisplayStyle.None;
    }

    protected void ShowMenuPanel()
    {
        _menuUI.style.display = DisplayStyle.Flex;
        _optionsUI.style.display = DisplayStyle.None;
        _trailerVideoUI.style.display = DisplayStyle.None;
    }

    protected void ShowTrailerVideoUIPanel()
    {
        _trailerVideoUI.style.display = DisplayStyle.Flex;
        _menuUI.style.display = DisplayStyle.None;
        _optionsUI.style.display = DisplayStyle.None;
    }
}