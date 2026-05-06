using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [Header("Tutorial UI")]
    [SerializeField] private Image _bgImage;
    [SerializeField] private Image _explainationImage;
    [SerializeField] private Image _tutorialPlayer;

    [Header("Tutorial UI Animation Settings")]
    [SerializeField] private float _bgUiFadeTime = .2f;
    [SerializeField] private float _bgUiMaxAlpha = .6f;
    [SerializeField] private float _explainationUiZoomTime = .5f;
    [SerializeField] private float _playerUiFadeTime = .2f;
    [SerializeField] private AnimationCurve _explainationUiZoomInCurve;
    [SerializeField] private AnimationCurve _explainationUiZoomOutCurve;

    private void Awake()
    {
        // reset
        _bgImage.gameObject.SetActive(false);
        _explainationImage.gameObject.SetActive(false);
        _tutorialPlayer.gameObject.SetActive(false);
    }

    void Start()
    {
        StartCoroutine(WaitTimer.WaitFor(3f, () => OnShowTutorial()));
    }

    private void OnContinueTutorial(InputValue val)
    {
        if (val.isPressed)
        {
            OnExitTutorial();   // reset exit tutorial
        }
    }

    private void OnShowTutorial(/*Sprite spriteToShow*/)
    {
        Time.timeScale = 0f;    // pause game time

        // show tutorial ui
        _bgImage.gameObject.SetActive(true);
        StartCoroutine(UIAnimation.FadeIn(_bgUiFadeTime, _bgUiMaxAlpha, _bgImage));

        _explainationImage.gameObject.SetActive(true);
        // _explainationImage.sprite = spriteToShow;
        StartCoroutine(UIAnimation.ZoomIn(_explainationUiZoomTime, _explainationImage, _explainationUiZoomInCurve));

        _tutorialPlayer.gameObject.SetActive(true);
        StartCoroutine(UIAnimation.FadeIn(_playerUiFadeTime, 1f, _tutorialPlayer));
    }

    private void OnExitTutorial()
    {
        Time.timeScale = 1f;    // resume game time

        // hide tutorial ui
        StartCoroutine(UIAnimation.FadeOut(_bgUiFadeTime, _bgUiMaxAlpha, _bgImage, () => _bgImage.gameObject.SetActive(false)));
        StartCoroutine(UIAnimation.ZoomOut(_explainationUiZoomTime, _explainationImage, _explainationUiZoomOutCurve, () => _explainationImage.gameObject.SetActive(false)));
        StartCoroutine(UIAnimation.FadeOut(_playerUiFadeTime, 1f, _tutorialPlayer, () => _tutorialPlayer.gameObject.SetActive(false)));
    }
}