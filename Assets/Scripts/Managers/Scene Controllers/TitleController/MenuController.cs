using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MenuController : TitleController
{
    [Header("Element Names")]
    [SerializeField] private string _startButton = "StartButton";
    [SerializeField] private string _optionsButton = "OptionsButton";
    [SerializeField] private string _playerNameInput = "NameInput";

    [Space(10)]
    [Header("Hand Detection")]
    [SerializeField] private HandDetection _handDetection;
    [SerializeField] private float _handDetectionStartDelay = 2f;
    [SerializeField] private string _leftHandElementName = "HandLeft";  // ui toolkit
    [SerializeField] private string _rightHandElementName = "HandRight";  // ui toolkit
    [SerializeField] private GameObject _leftHandMarker;    // actual hand sprites in world space
    [SerializeField] private GameObject _rightHandMarker;   // actual hand sprites in world space

    private Camera _mainCam;
    private TextField _playerName;
    private Image _leftHandImage;
    private Image _rightHandImage;

    private void OnEnable()
    {
        _handDetection.OnHandCheckStart += OnHandCheckStart;
    }

    protected override void Awake()
    {
        base.Awake();

        if (_handDetection == null)
            Debug.LogError("HandDetection reference is missing in MenuController!");
    }

    protected override void Start()
    {
        base.Start();

        _mainCam = Camera.main;

        Button startButton = MenuUI.Q<Button>(_startButton);
        Button optionsButton = MenuUI.Q<Button>(_optionsButton);
        _playerName = MenuUI.Q<TextField>(_playerNameInput);
        _leftHandImage = MenuUI.Q<Image>(_leftHandElementName);
        _rightHandImage = MenuUI.Q<Image>(_rightHandElementName);

        optionsButton.clicked += ShowOptionsPanel;

        // start hand detection
        StartCoroutine(WaitTimer.WaitFor(_handDetectionStartDelay, () =>
        {
            _handDetection.StartCheck();
        }));
    }

    private void LateUpdate()
    {
        _leftHandImage.style.display = _rightHandMarker.activeSelf ? DisplayStyle.Flex : DisplayStyle.None;
        _rightHandImage.style.display = _leftHandMarker.activeSelf ? DisplayStyle.Flex : DisplayStyle.None;

        if (!_leftHandMarker.activeSelf && !_rightHandMarker.activeSelf) return;

        // position the hand images on the screen according to the hand marker positions in world space
        // left hand marker corresponds to right hand image and vice versa
        Vector3 leftHandViewportPos = _mainCam.WorldToScreenPoint(_rightHandMarker.transform.position);
        Vector3 rightHandViewportPos = _mainCam.WorldToScreenPoint(_leftHandMarker.transform.position);

        // calculate the x pos and set it to the hand images, y pos is fixed in the center
        float leftHandX = leftHandViewportPos.x - _mainCam.pixelRect.width / 2;
        float rightHandX = rightHandViewportPos.x - _mainCam.pixelRect.width / 2;
        _leftHandImage.style.left = new(Length.Pixels(leftHandX));
        _rightHandImage.style.left = new(Length.Pixels(rightHandX));
    }

    // detected heart
    private void OnHandCheckStart()
    {
        // get name from input before going to next scene
        if (string.IsNullOrEmpty(_playerName.value))
        {
            _playerName.Focus();
            _handDetection.StartCheck();    // restart hand detection if name is not entered
            return;
        }

        PlayerDataManager.Instance.SetPlayerName(_playerName.value);
        GameManager.Instance.GoNextScene();
    }
}