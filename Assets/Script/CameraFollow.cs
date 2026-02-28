using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // 需要跟随的主角
    public float smoothTime = 0.15f;
    public float xOffset = 0f;    // 相机相对主角的水平偏移
    [Header("水平边界（可选）")]
    public bool useBounds = false;
    public float minX = -10f;
    public float maxX = 100f;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null) return;

        // 只在 X 轴上跟随主角，保持当前的 Y 和 Z
        var targetPos = new Vector3(
            target.position.x + xOffset,
            transform.position.y,
            transform.position.z
        );

        if (useBounds)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );
    }
}
