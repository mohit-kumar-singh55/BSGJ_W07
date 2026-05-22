using UnityEngine;

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
        // spawn fever mode text ui popup
        _uiManager.SpawnFeverModeTextPopup(_feverModeTextPrefab, _feverModeTextParent, _destroyfeverPopupDelay);

        // play fever sfx
        _audioManager.PlaySFX(SFX.Fever);
    }
}