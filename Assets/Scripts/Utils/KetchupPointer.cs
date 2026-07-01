using UnityEngine;

/// <summary>
/// ケチャップボトルの動作を管理するクラス
/// </summary>
public class KetchupPointer : MonoBehaviour
{
    [SerializeField] private GameObject _ketchupBottle;
    [SerializeField] private float _bottleOffsetFromCamera = .8f;

    private bool _isDrawing = false;

    void OnEnable()
    {
        // allow to draw
        PlayerPainting.OnPlayerEnterPainting += StartDrawing;
        PlayerPainting.OnPlayerExitPainting += StopDrawing;
    }

    void OnDisable()
    {
        // disallow to draw
        PlayerPainting.OnPlayerEnterPainting -= StartDrawing;
        PlayerPainting.OnPlayerExitPainting -= StopDrawing;
    }

    void Start()
    {
        if (_ketchupBottle == null)
        {
            Debug.LogError("Ketchup bottle GameObject is not assigned in the inspector.");
            enabled = false;
            return;
        }

        // Initially hide the ketchup bottle
        _ketchupBottle.SetActive(false);
    }

    private void Update()
    {
        if (!_isDrawing) return;

        // マウスの画面上での座標を取得する
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _bottleOffsetFromCamera; // カメラからボトルまでの距離
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // ケチャップボトルをマウス位置へ移動する
        _ketchupBottle.transform.position = worldPos;
    }

    private void StartDrawing()
    {
        _isDrawing = true;
        _ketchupBottle.SetActive(true);

        // hide cursor
        Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
    }

    private void StopDrawing()
    {
        _isDrawing = false;
        _ketchupBottle.SetActive(false);

        // show cursor
        Cursor.visible = true;
        // Cursor.lockState = CursorLockMode.None;
    }
}