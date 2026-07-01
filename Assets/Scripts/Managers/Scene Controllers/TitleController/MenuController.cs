using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// メニューUIのコントローラークラス
/// </summary>
[RequireComponent(typeof(TitleController))]
public class MenuController : MonoBehaviour
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
    [SerializeField] private GameObject _leftHandMarker;    // ワールド空間上の実際の手のスプライト
    [SerializeField] private GameObject _rightHandMarker;   // ワールド空間上の実際の手のスプライト

    private Camera _mainCam;
    private TextField _playerName;
    private Image _leftHandImage;
    private Image _rightHandImage;
    private TitleController _titleController;

    private void OnEnable()
    {
        _handDetection.OnHandCheckStart += OnHandCheckStart;
    }

    private void OnDisable()
    {
        _handDetection.OnHandCheckStart -= OnHandCheckStart;
    }

    private void Awake()
    {
        if (_handDetection == null)
            Debug.LogError("HandDetection reference is missing in MenuController!");
    }

    private void Start()
    {
        _mainCam = Camera.main;
        _titleController = GetComponent<TitleController>();

        Button startButton = _titleController.MenuUI.Q<Button>(_startButton);
        Button optionsButton = _titleController.MenuUI.Q<Button>(_optionsButton);
        _playerName = _titleController.MenuUI.Q<TextField>(_playerNameInput);
        _leftHandImage = _titleController.MenuUI.Q<Image>(_leftHandElementName);
        _rightHandImage = _titleController.MenuUI.Q<Image>(_rightHandElementName);

        optionsButton.clicked += _titleController.ShowOptionsPanel;

        // 手認識を開始する前に少し待つ
        StartCoroutine(WaitTimer.WaitFor(_handDetectionStartDelay, () =>
        {
            // 開始時に名前入力欄へフォーカスする
            _playerName.Focus();

            _handDetection.StartCheck();
        }));
    }

    // UI Toolkitの手画像をワールド空間の手マーカーと同期する
    private void LateUpdate()
    {
        _leftHandImage.style.display = _rightHandMarker.activeSelf ? DisplayStyle.Flex : DisplayStyle.None;
        _rightHandImage.style.display = _leftHandMarker.activeSelf ? DisplayStyle.Flex : DisplayStyle.None;

        if (!_leftHandMarker.activeSelf && !_rightHandMarker.activeSelf) return;

        // ワールド空間の手マーカー位置に合わせて、画面上の手画像を配置する
        // 左手マーカーは右手画像に対応し、右手マーカーは左手画像に対応する
        Vector3 leftHandViewportPos = _mainCam.WorldToScreenPoint(_rightHandMarker.transform.position);
        Vector3 rightHandViewportPos = _mainCam.WorldToScreenPoint(_leftHandMarker.transform.position);

        // 手画像に設定するX座標を計算する
        // Y座標は画面中央に固定する
        float leftHandX = leftHandViewportPos.x - _mainCam.pixelRect.width / 2;
        float rightHandX = rightHandViewportPos.x - _mainCam.pixelRect.width / 2;
        _leftHandImage.style.left = new(Length.Pixels(leftHandX));
        _rightHandImage.style.left = new(Length.Pixels(rightHandX));
    }

    // ハートの形が認識された
    private void OnHandCheckStart()
    {
        // 次のシーンへ進む前に入力欄から名前を取得する
        if (string.IsNullOrEmpty(_playerName.value))
        {
            _playerName.Focus();
            _handDetection.StartCheck();    // 名前が入力されていない場合は手認識を再開する
            return;
        }

        PlayerDataManager.Instance.SetPlayerName(_playerName.value);
        GameManager.Instance.GoNextScene();
    }
}