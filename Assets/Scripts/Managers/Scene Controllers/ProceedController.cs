using UnityEngine;
using UnityEngine.UIElements;

public class ProceedController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIDocument _uIPanel;

    [Header("Element Names")]
    [SerializeField] private string _proceedButtonName = "ProceedButton";

    private void Start()
    {
        VisualElement root = _uIPanel.rootVisualElement;

        Button proceedButton = root.Q<Button>(_proceedButtonName);
        proceedButton.clicked += GameManager.Instance.GoNextScene;
    }
}