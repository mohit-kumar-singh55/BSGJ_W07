using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum TutorialDialogStep
{
    NONE,
    NEW_MAID_TRAINING,
    CONTROLS_EXPLAINATION,
    DRAW_HEART,
    MOE_KYUN_EXPLAINATION,
    FEVER_MODE_EXPLAINATION,
    END_LINE
}

[Serializable]
public struct TutorialDialog
{
    public TutorialDialogStep step;
    public Sprite explainationSprite_EN;
    public Sprite explainationSprite_JP;
    [TextArea] public string explainationText_EN;
    [TextArea] public string explainationText_JP;
}

public class TutorialController : MonoBehaviour
{
    [Header("Tutorial UI")]
    [SerializeField] private Image _bgImage;
    [SerializeField] private Image _explainationImage;
    [SerializeField] private Image _tutorialPlayer;
    [SerializeField] private Image _tutorialTextParentBG;
    [SerializeField] private TMP_Text _tutorialText;

    [Space(10)]
    [Header("Tutorial UI Animation Settings")]
    [SerializeField] private float _bgUiFadeTime = .2f;
    [SerializeField] private float _bgUiMaxAlpha = .6f;
    [SerializeField] private float _explainationUiZoomTime = .5f;
    [SerializeField] private float _playerUiFadeTime = .2f;
    [SerializeField] private AnimationCurve _explainationUiZoomInCurve;
    [SerializeField] private AnimationCurve _explainationUiZoomOutCurve;

    [Space(10)]
    [Header("Tutorial Dialog")]
    [SerializeField] private List<TutorialDialog> _tutorialDialogs;

    [Space(10)]
    [SerializeField] private float _goNextSceneDelay = 1f;

    private GameManager _gameManager;
    private TutorialDialogStep _currentTutorialDialogStep;
    private Dictionary<TutorialDialogStep, TutorialDialog> _tutorialDialogDict = new();

    private void OnEnable()
    {
        CustomerInService.OnCustomerEnterInService += OnMaidTrainingStart;
        PlayerDoingMoeMoe.OnMoeMoeStarted += OnMoeKyunExplainationStart;
        PlayerDoingMoeMoe.OnMoeMoeCompleted += OnFeverModeExplainationStart;
    }

    private void OnDisable()
    {
        CustomerInService.OnCustomerEnterInService -= OnMaidTrainingStart;
        PlayerDoingMoeMoe.OnMoeMoeStarted -= OnMoeKyunExplainationStart;
        PlayerDoingMoeMoe.OnMoeMoeCompleted -= OnFeverModeExplainationStart;
    }

    private void Awake()
    {
        // reset
        _bgImage.gameObject.SetActive(false);
        _explainationImage.gameObject.SetActive(false);
        _tutorialPlayer.gameObject.SetActive(false);
        _tutorialTextParentBG.gameObject.SetActive(false);

        // initialize tutorial dialog dictionary
        foreach (var dialog in _tutorialDialogs)
            _tutorialDialogDict[dialog.step] = dialog;
    }

    void Start()
    {
        _currentTutorialDialogStep = TutorialDialogStep.NONE;
        _gameManager = GameManager.Instance;
    }

    // Input System callback
    private void OnContinueTutorial(InputValue val)
    {
        if (!val.isPressed) return;

        // current tutorial dialog at the time of clicking
        switch (_currentTutorialDialogStep)
        {
            case TutorialDialogStep.NEW_MAID_TRAINING:
                OnControlExplainationStart();
                break;
            case TutorialDialogStep.CONTROLS_EXPLAINATION:
                OnDrawHeartStart();
                break;
            case TutorialDialogStep.DRAW_HEART:
                HideAllTutorialUI();
                break;
            case TutorialDialogStep.MOE_KYUN_EXPLAINATION:
                HideAllTutorialUI();
                break;
            case TutorialDialogStep.FEVER_MODE_EXPLAINATION:
                OnEndLineExplainationStart();
                break;
            case TutorialDialogStep.END_LINE:
                HideAllTutorialUI();
                // ** tutorial completed, reset the score and go the next scene **
                StartCoroutine(WaitTimer.WaitFor(_goNextSceneDelay, () =>
                {
                    PlayerDataManager.Instance.ResetScore();
                    _gameManager.GoNextScene();
                }));
                break;
            default:
                break;
        }
    }

    private void OnMaidTrainingStart()
    {
        Time.timeScale = 0f;    // pause game time

        _currentTutorialDialogStep = TutorialDialogStep.NEW_MAID_TRAINING;
        TutorialDialog _currentTutorialDialog = _tutorialDialogDict[_currentTutorialDialogStep];

        switch (_gameManager.CurrentLanguage)
        {
            case LANG.ENGLISH:
                _tutorialText.text = _currentTutorialDialog.explainationText_EN;
                break;
            case LANG.JAPANESE:
                _tutorialText.text = _currentTutorialDialog.explainationText_JP;
                break;
        }

        ShowTutorialBG(true);
        ShowTutorialPlayer(true);
        ShowTutorialText(true);
    }

    private void OnControlExplainationStart()
    {
        // Time.timeScale = 0f;    // pause game time

        _currentTutorialDialogStep = TutorialDialogStep.CONTROLS_EXPLAINATION;
        TutorialDialog _currentTutorialDialog = _tutorialDialogDict[_currentTutorialDialogStep];

        switch (_gameManager.CurrentLanguage)
        {
            case LANG.ENGLISH:
                _tutorialText.text = _currentTutorialDialog.explainationText_EN;
                _explainationImage.sprite = _currentTutorialDialog.explainationSprite_EN;
                break;
            case LANG.JAPANESE:
                _tutorialText.text = _currentTutorialDialog.explainationText_JP;
                _explainationImage.sprite = _currentTutorialDialog.explainationSprite_JP;
                break;
        }

        ShowTutorialExplanation(true);
    }

    private void OnDrawHeartStart()
    {
        // Time.timeScale = 0f;    // pause game time

        _currentTutorialDialogStep = TutorialDialogStep.DRAW_HEART;
        TutorialDialog _currentTutorialDialog = _tutorialDialogDict[_currentTutorialDialogStep];

        switch (_gameManager.CurrentLanguage)
        {
            case LANG.ENGLISH:
                _tutorialText.text = _currentTutorialDialog.explainationText_EN;
                break;
            case LANG.JAPANESE:
                _tutorialText.text = _currentTutorialDialog.explainationText_JP;
                break;
        }
    }

    private void OnMoeKyunExplainationStart()
    {
        Time.timeScale = 0f;    // pause game time

        _currentTutorialDialogStep = TutorialDialogStep.MOE_KYUN_EXPLAINATION;
        TutorialDialog _currentTutorialDialog = _tutorialDialogDict[_currentTutorialDialogStep];

        switch (_gameManager.CurrentLanguage)
        {
            case LANG.ENGLISH:
                _tutorialText.text = _currentTutorialDialog.explainationText_EN;
                _explainationImage.sprite = _currentTutorialDialog.explainationSprite_EN;
                break;
            case LANG.JAPANESE:
                _tutorialText.text = _currentTutorialDialog.explainationText_JP;
                _explainationImage.sprite = _currentTutorialDialog.explainationSprite_JP;
                break;
        }

        ShowTutorialBG(true);
        ShowTutorialExplanation(true);
        ShowTutorialPlayer(true);
        ShowTutorialText(true);
    }

    private void OnFeverModeExplainationStart()
    {
        Time.timeScale = 0f;    // pause game time

        _currentTutorialDialogStep = TutorialDialogStep.FEVER_MODE_EXPLAINATION;
        TutorialDialog _currentTutorialDialog = _tutorialDialogDict[_currentTutorialDialogStep];

        switch (_gameManager.CurrentLanguage)
        {
            case LANG.ENGLISH:
                _tutorialText.text = _currentTutorialDialog.explainationText_EN;
                _explainationImage.sprite = _currentTutorialDialog.explainationSprite_EN;
                break;
            case LANG.JAPANESE:
                _tutorialText.text = _currentTutorialDialog.explainationText_JP;
                _explainationImage.sprite = _currentTutorialDialog.explainationSprite_JP;
                break;
        }

        ShowTutorialBG(true);
        ShowTutorialExplanation(true);
        ShowTutorialPlayer(true);
        ShowTutorialText(true);
    }

    private void OnEndLineExplainationStart()
    {
        Time.timeScale = 0f;    // pause game time

        _currentTutorialDialogStep = TutorialDialogStep.END_LINE;
        TutorialDialog _currentTutorialDialog = _tutorialDialogDict[_currentTutorialDialogStep];

        switch (_gameManager.CurrentLanguage)
        {
            case LANG.ENGLISH:
                _tutorialText.text = _currentTutorialDialog.explainationText_EN;
                break;
            case LANG.JAPANESE:
                _tutorialText.text = _currentTutorialDialog.explainationText_JP;
                break;
        }

        ShowTutorialExplanation(false);
    }

    private void HideAllTutorialUI()
    {
        Time.timeScale = 1f;
        ShowTutorialBG(false);
        ShowTutorialExplanation(false);
        ShowTutorialPlayer(false);
        ShowTutorialText(false);
    }

    private void ShowTutorialBG(bool show = true)
    {
        if (show)
        {
            _bgImage.gameObject.SetActive(true);
            StartCoroutine(UIAnimation.FadeIn(_bgUiFadeTime, _bgUiMaxAlpha, _bgImage));
        }
        else
            StartCoroutine(UIAnimation.FadeOut(_bgUiFadeTime, _bgUiMaxAlpha, _bgImage, () => _bgImage.gameObject.SetActive(false)));
    }

    private void ShowTutorialExplanation(bool show = true)
    {
        if (show)
        {
            _explainationImage.gameObject.SetActive(true);
            StartCoroutine(UIAnimation.ZoomIn(_explainationUiZoomTime, _explainationImage, _explainationUiZoomInCurve));
        }
        else
            StartCoroutine(UIAnimation.ZoomOut(_explainationUiZoomTime, _explainationImage, _explainationUiZoomOutCurve, () => _explainationImage.gameObject.SetActive(false)));
    }

    private void ShowTutorialPlayer(bool show = true)
    {
        if (show)
        {
            _tutorialPlayer.gameObject.SetActive(true);
            StartCoroutine(UIAnimation.FadeIn(_playerUiFadeTime, 1f, _tutorialPlayer));
        }
        else
            StartCoroutine(UIAnimation.FadeOut(_playerUiFadeTime, 1f, _tutorialPlayer, () => _tutorialPlayer.gameObject.SetActive(false)));
    }

    private void ShowTutorialText(bool show = true)
    {
        if (show)
        {
            _tutorialTextParentBG.gameObject.SetActive(true);
            StartCoroutine(UIAnimation.ZoomIn(_explainationUiZoomTime, _tutorialTextParentBG, _explainationUiZoomInCurve));
        }
        else
            StartCoroutine(UIAnimation.ZoomOut(_explainationUiZoomTime, _tutorialTextParentBG, _explainationUiZoomOutCurve, () => _tutorialTextParentBG.gameObject.SetActive(false)));
    }
}