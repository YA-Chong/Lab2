using UnityEngine;

/// <summary>
/// 挂在火球 Prefab 上。火球水平匀速飞行，靠重力产生抛物线；碰到地面弹起，碰到墙/敌人销毁。
/// Prefab 要求：Rigidbody2D（Dynamic，开启重力）、Collider2D（不勾 Is Trigger）、Physics Material 2D（Bounciness=1，Friction=0）
/// </summary>
public class FireballController : MonoBehaviour
{
    [Header("飞行")]
    [Tooltip("水平飞行速度")]
    public float speed = 10f;
    [Tooltip("飞行超过此距离后自动销毁")]
    public float maxDistance = 15f;
    [Tooltip("判定为碰到地面可弹起的法线 Y 阈值；低于此值（接近水平）视为撞墙销毁")]
    [Range(0.3f, 1f)]
    public float groundNormalThreshold = 0.5f;

    private int direction = 1;
    private Vector2 startPos;
    private Rigidbody2D rb;

    public void Init(int dir)
    {
        direction = dir;
        startPos = transform.position;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        // 保持水平速度恒定，Y 方向由物理（重力 + 弹跳）决定
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        // 超出最大距离自动销毁
        if (Vector2.Distance(startPos, transform.position) >= maxDistance)
            Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 碰到敌人（包括龟壳）：直接击杀，销毁火球
        var monster = collision.gameObject.GetComponent<MonsterController>();
        if (monster != null)
        {
            monster.KillByFireball();
            Destroy(gameObject);
            return;
        }

        // 根据碰撞法线判断是"地面弹起"还是"撞墙销毁"
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < groundNormalThreshold)
            {
                // 法线 Y 较小（接近水平）= 撞到侧面墙 → 销毁
                Destroy(gameObject);
                return;
            }
            // 法线 Y 较大（接近竖直）= 碰到地面 → 让 Physics Material 的弹跳处理，不销毁
        }
    }
}
