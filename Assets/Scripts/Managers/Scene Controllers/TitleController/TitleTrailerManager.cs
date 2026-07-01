using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Video;

/// <summary>
/// タイトル画面のトレーラー動画を管理するコントローラークラス
/// </summary>
[RequireComponent(typeof(TitleController))]
public class TrailerVideoManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private float _startVideoAfterInactivity = 30f;

    private TitleController _titleController;
    private bool _isPlaying = false;
    private float _lastTimePressedBefore = 0f;  // 最後に「何かキー」が押された時刻

    private void OnEnable()
    {
        InputSystem.onEvent += OnEvent;
    }

    private void OnDisable()
    {
        InputSystem.onEvent -= OnEvent;
    }

    private void Start()
    {
        _titleController = GetComponent<TitleController>();

        _lastTimePressedBefore = 0;
        _videoPlayer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_isPlaying) return;

        // 一定時間操作がない場合は動画を再生する
        if (!_isPlaying && _lastTimePressedBefore >= _startVideoAfterInactivity)
        {
            _isPlaying = true;
            _videoPlayer.gameObject.SetActive(true);
            _videoPlayer.Play();
            _titleController.ShowTrailerVideoUIPanel();
        }
        else _lastTimePressedBefore += Time.unscaledDeltaTime;
    }

    private void OnEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (eventPtr.IsA<StateEvent>() || eventPtr.IsA<DeltaStateEvent>())
        {
            // いずれかのボタンが押されたら時間をリセットする
            _lastTimePressedBefore = 0;

            // 動画を停止する
            if (_isPlaying)
            {
                _isPlaying = false;
                _videoPlayer.Stop();
                _videoPlayer.gameObject.SetActive(false);
                _titleController.ShowMenuPanel();
            }
        }
    }
}