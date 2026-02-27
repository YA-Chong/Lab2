using UnityEngine;

/// <summary>
/// 通用怪物：移动、巡逻、被踩后的反应。挂在敌人 Prefab 上，在 Inspector 里配置巡逻范围和被踩行为（动画、是否销毁等）。
/// </summary>
public class MonsterController : MonoBehaviour
{
    [Header("移动")]
    public float speed = 2f;
    [Tooltip("初始朝向：-1 向左，1 向右")]
    public int initialDirection = -1;

    [Header("巡逻范围（可选）")]
    [Tooltip("勾选后，怪物只在 leftBound 与 rightBound 之间来回走；不勾则仅靠撞墙掉头")]
    public bool usePatrolRange = false;
    [Tooltip("巡逻左边界（世界坐标 X）")]
    public float leftBound = 0f;
    [Tooltip("巡逻右边界（世界坐标 X）")]
    public float rightBound = 5f;

    [Header("被踩")]
    [Tooltip("被踩时触发的 Animator Trigger 名，不填则不播动画")]
    public string stompAnimTrigger = "Die";
    [Tooltip("被踩后是否销毁物体")]
    public bool destroyOnStomp = true;
    [Tooltip("销毁前延迟（秒）")]
    public float stompDestroyDelay = 0.5f;

    private int direction;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private bool stomped;

    /// <summary> 为 false 时不再移动（被踩、变壳等） </summary>
    [HideInInspector]
    public bool movementEnabled = true;

    private void Start()
    {
        direction = initialDirection;
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    /// <summary> 被主角踩头时由 MarioCombat 调用 </summary>
    public void Stomped()
    {
        if (stomped) return;
        stomped = true;
        movementEnabled = false;

        if (anim != null && !string.IsNullOrEmpty(stompAnimTrigger))
            anim.SetTrigger(stompAnimTrigger);

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (destroyOnStomp)
            Destroy(gameObject, stompDestroyDelay);
    }

    private void Update()
    {
        if (!movementEnabled) return;

        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        if (spriteRenderer != null)
            spriteRenderer.flipX = (direction == -1);

        if (usePatrolRange)
        {
            float x = transform.position.x;
            if (x <= leftBound && direction < 0) direction = 1;
            else if (x >= rightBound && direction > 0) direction = -1;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!movementEnabled || stomped) return;
        direction *= -1;
    }

    private void OnDrawGizmosSelected()
    {
        if (!usePatrolRange) return;
        float y = transform.position.y;
        Vector3 left = new Vector3(leftBound, y, 0f);
        Vector3 right = new Vector3(rightBound, y, 0f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(left, right);
        Gizmos.DrawWireSphere(left, 0.15f);
        Gizmos.DrawWireSphere(right, 0.15f);
    }
}
