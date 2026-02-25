using UnityEngine;
using System.Collections;

public class MarioController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 8f;
    public float jumpForce = 16f;

    [Header("状态引用")]
    public AnimatorOverrideController bigMarioController;
    private RuntimeAnimatorController smallMarioController;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D col; // 引用碰撞盒以调整尺寸

    [Header("检测设置")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float checkRadius = 0.2f;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool isBig = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>(); // 获取碰撞盒组件
        smallMarioController = anim.runtimeAnimatorController;
    }

    void Update()
    {
        // 1. 地面检测
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // 2. 左右移动
        float moveInput = Input.GetAxisRaw("Horizontal");
        // 使用 Unity 6 推荐的 linearVelocity
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // 3. 翻转角色 (解决变小和方向回弹问题)
        // 只有在 moveInput 不为 0 时才更新 flipX，这样停止时会保留最后的方向
        // 3. 翻转角色 (增强版：基于实际物理速度判断，彻底杜绝回弹)
        // 使用 rb.linearVelocity.x 替代 moveInput，这样只要马里奥还有惯性或处于移动状态，方向就不会错
        if (rb.linearVelocity.x > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (rb.linearVelocity.x < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
        // 当速度几乎为 0 时，不执行任何操作，这样他就会停留在最后的方向

        // 4. 跳跃
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 5. 变身测试 (按下 E 键)
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(PowerUpSequence());
        }

        // --- 更新 Animator 参数 ---
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("verticalVelocity", rb.linearVelocity.y);
    }

    IEnumerator PowerUpSequence()
    {
        anim.SetTrigger("onPowerUp");

        // 等待 0.5 秒（变身动画播放到一半时切换）
        yield return new WaitForSeconds(0.5f);

        isBig = !isBig;
        anim.runtimeAnimatorController = isBig ? bigMarioController : smallMarioController;

        // --- 核心修改：针对 Center 中心点调整碰撞盒 ---
        // 设小马高度为 Hs，大马为 Hb，为了让脚底位置不变，偏移量 Offset 需上移：
        // Offset = (Hb - Hs) / 2
        //if (isBig)
        //{
        //    col.size = new Vector2(0.8f, 1.2f);   // 假设大马高度为 1.8
        //    col.offset = new Vector2(0f, 0.1f);  // (1.8 - 1.0) / 2 = 0.4
        //}
        //else
        //{
        //    col.size = new Vector2(0.8f, 1.0f);   // 恢复小马高度 1.0
        //    col.offset = new Vector2(0f, 0f);    // 回到中心
        //}
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}