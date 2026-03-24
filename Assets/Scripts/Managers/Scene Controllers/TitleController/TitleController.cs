using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public abstract class TitleController : MonoBehaviour
{
    private UIDocument _uiPanel;
    private VisualElement _menuUI;
    private VisualElement _optionsUI;

    protected VisualElement MenuUI => _menuUI;
    protected VisualElement OptionsUI => _optionsUI;

    private const string MENU_UI_NAME = "Menu";
    private const string OPTIONS_UI_NAME = "Options";

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

        // initially show menu only
        ShowMenuPanel();
    }

    protected void ShowOptionsPanel()
    {
        _optionsUI.style.display = DisplayStyle.Flex;
        _menuUI.style.display = DisplayStyle.None;
    }

    protected void ShowMenuPanel()
    {
        _menuUI.style.display = DisplayStyle.Flex;
        _optionsUI.style.display = DisplayStyle.None;
    }
}