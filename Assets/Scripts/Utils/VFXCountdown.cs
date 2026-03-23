using System;
using System.Collections;
using UnityEngine;

public class VFXCountdown : MonoBehaviour
{
    [Tooltip("Should be in incremental order (0, 1, 2, 3...) and match the countdown seconds")]
    [SerializeField] private GameObject[] _countdownVFXPrefabs;
    [SerializeField] private float _countdownInterval = 2f;

    [Space(10)]
    [Header("Placement Settings")]
    [SerializeField] private Vector3 _OffsetFromCamera = new(.2f, 0f, 2f);

    private Camera _mainCamera;
    private bool _isCounting = false;

    public float CountdownInterval { set { _countdownInterval = value; } }

    private void Start() => _mainCamera = Camera.main;

    private void MoveToAndFaceCamera()
    {
        Vector3 pos = _mainCamera.transform.position;
        pos += _mainCamera.transform.right * _OffsetFromCamera.x;
        pos += _mainCamera.transform.up * _OffsetFromCamera.y;
        pos += _mainCamera.transform.forward * _OffsetFromCamera.z;

        // move and face camera
        transform.position = pos;
        transform.LookAt(_mainCamera.transform);
    }

    void LateUpdate()
    {
        if (_isCounting) MoveToAndFaceCamera();
    }

    public void StartCountdown(Action onComplete = null)
    {
        StopAllCoroutines();
        StartCoroutine(ShowCountdown(onComplete));
    }

    private IEnumerator ShowCountdown(Action onComplete = null)
    {
        _isCounting = true;
        int countdownIndex = _countdownVFXPrefabs.Length - 1;

        while (countdownIndex >= 0)
        {
            Instantiate(_countdownVFXPrefabs[countdownIndex], transform.position, Quaternion.identity);
            yield return new WaitForSeconds(_countdownInterval);

            countdownIndex--;
        }

        _isCounting = false;
        onComplete?.Invoke();
    }
}