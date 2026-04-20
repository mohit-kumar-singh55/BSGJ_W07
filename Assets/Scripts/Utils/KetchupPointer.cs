using UnityEngine;

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

        // Get the position of the mouse in world space
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = _bottleOffsetFromCamera; // distance of the bottle from the camera
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // Move the ketchup bottle to the mouse position
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