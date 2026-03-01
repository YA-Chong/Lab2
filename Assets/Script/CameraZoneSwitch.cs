using UnityEngine;

public class CameraZoneSwitch : MonoBehaviour
{
    [Header("Drag the Cinemachine Camera *component* here")]
    public MonoBehaviour mainCam;   // 拖 CM_Main 上的 Cinemachine Camera 组件
    public MonoBehaviour dropCam;   // 拖 CM_Drop 上的 Cinemachine Camera 组件

    [Header("Priority")]
    public int mainPriority = 10;
    public int dropPriorityWhenActive = 20;

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private bool IsMario(Collider2D other)
    {
        // 碰到 Mario 或 Mario 的子物体都算
        return other.CompareTag("Mario") || other.GetComponentInParent<MarioController>() != null;
    }

    private void SetPriority(MonoBehaviour camBehaviour, int value)
    {
        if (camBehaviour == null) return;

        var t = camBehaviour.GetType();

        // 优先找属性 Priority
        var prop = t.GetProperty("Priority");
        if (prop != null)
        {
            prop.SetValue(camBehaviour, value);
            return;
        }

        // 再找字段 Priority
        var field = t.GetField("Priority");
        if (field != null)
        {
            field.SetValue(camBehaviour, value);
            return;
        }

        Debug.LogWarning($"No Priority member found on {t.Name}. Make sure you dragged the Cinemachine Camera component, not the GameObject.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsMario(other)) return;

        SetPriority(mainCam, mainPriority);
        SetPriority(dropCam, dropPriorityWhenActive);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsMario(other)) return;

        SetPriority(dropCam, 0);
        SetPriority(mainCam, mainPriority);
    }
}