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
    [Tooltip("被踩时触发的 Animator Trigger 名，不填则不播动画（变壳时不使用）")]
    public string stompAnimTrigger = "Die";
    [Tooltip("被踩后是否销毁物体（乌龟勾选「变壳」时此项不生效）")]
    public bool destroyOnStomp = true;
    [Tooltip("销毁前延迟（秒）")]
    public float stompDestroyDelay = 0.5f;

    [Header("龟壳（仅乌龟勾选）")]
    [Tooltip("勾选后，被踩时变为壳并沿原行进方向滑动，不销毁")]
    public bool becomeShellOnStomp = false;
    [Tooltip("壳滑动速度")]
    public float shellSlideSpeed = 12f;
    [Tooltip("壳滑动超过此距离后销毁")]
    public float shellMaxDistance = 20f;
    [Tooltip("变壳时设置的 Animator Bool 参数名")]
    public string shellAnimBool = "isShell";

    private int direction;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Rigidbody2D rb;
    private bool stomped;
    private bool isShell;
    private float shellSlideDistanceTraveled;

    /// <summary> 为 false 时不再移动（被踩、变壳等） </summary>
    [HideInInspector]
    public bool movementEnabled = true;

    private void Start()
    {
        direction = initialDirection;
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 被火球击中时调用。
    /// 已是壳 → 直接给分并销毁；未变壳 → 和踩头效果一致（蘑菇死亡，乌龟变壳）
    /// </summary>
    public void KillByFireball()
    {
        if (isShell)
        {
            var scoreComp = GetComponent<MonsterStompScore>();
            if (scoreComp != null) scoreComp.GrantStompScore();
            Destroy(gameObject);
            return;
        }

        // 未变壳时，效果等同踩头（蘑菇死亡动画销毁，乌龟变壳）
        Stomped();
    }

    /// <summary> 被主角踩头时由 MarioCombat 调用 </summary>
    public void Stomped()
    {
        if (stomped) return;
        stomped = true;
        movementEnabled = false;

        var scoreComp = GetComponent<MonsterStompScore>();
        if (scoreComp != null) scoreComp.GrantStompScore();

        if (becomeShellOnStomp)
        {
            isShell = true;
            if (anim != null && !string.IsNullOrEmpty(shellAnimBool))
                anim.SetBool(shellAnimBool, true);
            if (spriteRenderer != null) spriteRenderer.flipX = false;

            if (rb != null)
            {
                // 保持 Dynamic，让物理引擎真正阻挡碰撞；关掉重力，只水平滑动
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.gravityScale = 0f;
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb.linearVelocity = new Vector2(direction * shellSlideSpeed, 0f);
            }
            return;
        }

        if (anim != null && !string.IsNullOrEmpty(stompAnimTrigger))
            anim.SetTrigger(stompAnimTrigger);

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (destroyOnStomp)
            Destroy(gameObject, stompDestroyDelay);
    }

    private void FixedUpdate()
    {
        if (isShell && rb != null)
        {
            // 保持速度恒定（Dynamic 刚体受摩擦力等可能减速）
            rb.linearVelocity = new Vector2(direction * shellSlideSpeed, rb.linearVelocity.y);

            shellSlideDistanceTraveled += shellSlideSpeed * Time.fixedDeltaTime;
            if (shellSlideDistanceTraveled >= shellMaxDistance)
                Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (isShell) return;

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
        if (isShell)
        {
            if (collision.gameObject.CompareTag("Mario"))
            {
                var combat = collision.gameObject.GetComponent<MarioCombat>();
                if (combat != null) combat.OnHitByEnemy();
            }
            // 撞墙或撞到马里奥后反向，更新速度方向
            direction *= -1;
            if (rb != null)
                rb.linearVelocity = new Vector2(direction * shellSlideSpeed, rb.linearVelocity.y);
            return;
        }

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
