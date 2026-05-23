using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Video;

public class TrailerVideoManager : TitleController
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private float _startVideoAfterInactivity = 30f;

    private bool _isPlaying = false;
    private float _lastTimePressedBefore = 0f;  // last time "any" button was pressed

    private void OnEnable()
    {
        InputSystem.onEvent += OnEvent;
    }

    private void OnDisable()
    {
        InputSystem.onEvent -= OnEvent;
    }

    protected override void Start()
    {
        base.Start();

        _lastTimePressedBefore = 0;
        _videoPlayer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_isPlaying) return;

        // play video if no activity for the given time
        if (!_isPlaying && _lastTimePressedBefore >= _startVideoAfterInactivity)
        {
            _isPlaying = true;
            _videoPlayer.gameObject.SetActive(true);
            _videoPlayer.Play();
            ShowTrailerVideoUIPanel();
        }
        else _lastTimePressedBefore += Time.unscaledDeltaTime;
    }

    private void OnEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (eventPtr.IsA<StateEvent>() || eventPtr.IsA<DeltaStateEvent>())
        {
            //     // reset time if pressed any button
            _lastTimePressedBefore = 0;

            // stop video
            if (_isPlaying)
            {
                _isPlaying = false;
                _videoPlayer.Stop();
                _videoPlayer.gameObject.SetActive(false);
                ShowMenuPanel();
            }
        }
    }
}