using UnityEngine;

/// <summary>
/// FeverEffectsはフィーバーモードのエフェクトを管理するクラス
/// フィーバーモードが開始された際に、UIのポップアップ表示
/// </summary>
public class FeverEffects : MonoBehaviour
{
    [Header("Fever Mode Text Popup")]
    [SerializeField] private GameObject _feverModeTextPrefab;
    [SerializeField] private Transform _feverModeTextParent;
    [SerializeField] private float _destroyfeverPopupDelay = 1f;

    private UIManager _uiManager;
    private AudioManager _audioManager;

    private void Start()
    {
        _uiManager = UIManager.Instance;
        _audioManager = AudioManager.Instance;
    }

    private void OnEnable()
    {
        FeverMode.OnFeverModeActivated += OnFeverModeStarted;
    }

    private void OnDisable()
    {
        FeverMode.OnFeverModeActivated -= OnFeverModeStarted;
    }

    private void OnFeverModeStarted()
    {
        // フィーバーモードのテキストUIポップアップを生成する
        _uiManager.SpawnFeverModeTextPopup(_feverModeTextPrefab, _feverModeTextParent, _destroyfeverPopupDelay);

        // play fever sfx
        _audioManager.PlaySFX(SFX.Fever);
    }
}