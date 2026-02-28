using UnityEngine;

public class MarioMovement : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 6f;

    [Header("Jump")]
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;          // 地面检测点（放在脚底）
    public float groundCheckRadius = 0.15f; // 检测半径
    public LayerMask groundLayer;           // 地面Layer

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1) 读取左右输入（A/D 或 ←/→）
        moveInput = Input.GetAxisRaw("Horizontal");

        // 2) 地面检测
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        // 3) 跳跃（空格）
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void FixedUpdate()
    {
        // 4) 水平移动（物理更新里做）
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // 5) 转向（让角色面向移动方向）
        if (moveInput > 0.01f) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < -0.01f) transform.localScale = new Vector3(-1, 1, 1);
    }

    // 在Scene里画出地面检测范围（辅助调试）
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
