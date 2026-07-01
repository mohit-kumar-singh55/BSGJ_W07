using UnityEngine;

/// <summary>
/// このクラスは、オブジェクトが常にカメラの方向を向くようにするビルボード効果を実装します。
/// </summary>
public class Billboarding : MonoBehaviour
{
    private Transform _mainCam;

    void Start()
    {
        _mainCam = Camera.main.transform;
    }

    void LateUpdate()
    {
        // Look at the camera
        transform.LookAt(transform.position + _mainCam.rotation * Vector3.forward, _mainCam.rotation * Vector3.up);
    }
}