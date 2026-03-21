using UnityEngine;

public class TwoHandGizmoDrawer : MonoBehaviour
{
    [SerializeField] private HandDataReceiver receiver;
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float radius = 0.05f;
    [SerializeField] private Color leftColor = Color.cyan;
    [SerializeField] private Color rightColor = Color.magenta;

    private void OnDrawGizmos()
    {
        if (receiver == null || receiver.handPos == null) return;
        if (receiver.handPos.Length < 2) return;
        if (targetCamera == null) return;

        DrawHand(receiver.handPos[0], leftColor);
        DrawHand(receiver.handPos[1], rightColor);
    }

    private void DrawHand(Vector3 mpPos, Color color)
    {
        if (mpPos == Vector3.zero) return;

        Vector3 worldPos = ConvertToWorld(mpPos);

        Gizmos.color = color;
        Gizmos.DrawSphere(worldPos, radius);
    }

    private Vector3 ConvertToWorld(Vector3 mpPos)
    {
        Vector3 screen = new Vector3(
            mpPos.x * Screen.width,
            (1f - mpPos.y) * Screen.height,
            targetCamera.nearClipPlane + 0.5f
        );

        return targetCamera.ScreenToWorldPoint(screen);
    }
}