using UnityEngine;

public class GoombaController : MonoBehaviour
{
    public float speed = 2f;
    private int direction = -1; // 默认先往左走
    private bool isDead = false;

    private SpriteRenderer spriteRenderer;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDead) return;

        // 1. 移动
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // 2. 这里的翻转逻辑：因为默认朝右，如果往左走(direction为-1)，就需要翻转
        spriteRenderer.flipX = (direction == -1);

        // 模拟按下 B 键变扁
        if (Input.GetKeyDown(KeyCode.B))
        {
            Stomped();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 撞到障碍物调头
        direction *= -1;
    }

    void Stomped()
    {
        isDead = true;
        anim.SetTrigger("Die");

        // 核心：禁用碰撞体，防止它挡路或继续检测碰撞
        GetComponent<Collider2D>().enabled = false;

        // 核心：0.5秒后销毁物体
        Destroy(gameObject, 0.5f);
    }
}