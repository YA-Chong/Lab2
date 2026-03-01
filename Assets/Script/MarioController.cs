using UnityEngine;
using System.Collections;

public class MarioController : MonoBehaviour
{
    [Header("???????")]
    public float moveSpeed = 8f;
    public float jumpForce = 16f;

    [Header("状态引用")]
    public AnimatorOverrideController bigMarioController;
    private RuntimeAnimatorController smallMarioController;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D col;

    [Header("大马里奥持续时间")]
    [Tooltip("变大后持续的秒数，0 表示永久")]
    public float bigMarioDuration = 8f;
    private Coroutine bigMarioTimerCoroutine;

    [Header("火球")]
    [Tooltip("火球 Prefab（需挂 FireballController，Collider2D 勾 Is Trigger）")]
    public GameObject fireballPrefab;
    [Tooltip("发射按键")]
    public KeyCode fireKey = KeyCode.Z;
    [Tooltip("同时存在的最大火球数")]
    public int maxFireballs = 2;
    private int activeFireballs;

    [Header("???????")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float checkRadius = 0.2f;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool isBig = false;
    private bool inPowerUpSequence;

    /// <summary> ????????????????????????????? true ???????????????? </summary>
    public bool IsBig => isBig;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>(); // ???????????
        smallMarioController = anim.runtimeAnimatorController;
    }

    void Update()
    {
        // 1. ??????
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // 2. ???????
        float moveInput = Input.GetAxisRaw("Horizontal");
        // ??? Unity 6 ????? linearVelocity
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // 3. ?????? (??????????????????)
        // ????? moveInput ??? 0 ?????? flipX????????????????????
        // 3. ?????? (??????????????????????????????????)
        // ??? rb.linearVelocity.x ??? moveInput???????????????????????????????????????
        if (rb.linearVelocity.x > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (rb.linearVelocity.x < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
        // ????????? 0 ??????????????????????????????????????

        // 4. ???
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            // 【新增音效】播放跳跃声音
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySFX(AudioManager.Instance.jumpClip);
        }

        // 5. 变身测试 (按下 E 键)
        if (Input.GetKeyDown(KeyCode.E) && !inPowerUpSequence)
        {
            StartCoroutine(PowerUpSequence());
        }

        // 6. 发射火球（仅大马里奥）
        if (isBig && Input.GetKeyDown(fireKey) && fireballPrefab != null && activeFireballs < maxFireballs)
        {
            ShootFireball();
        }

        // --- ???? Animator ???? ---
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("verticalVelocity", rb.linearVelocity.y);
    }

    IEnumerator PowerUpSequence()
    {
        inPowerUpSequence = true;
        anim.SetTrigger("onPowerUp");

        // 【新增音效】播放吃蘑菇变大声音
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.powerUpClip);

        yield return new WaitForSeconds(0.5f);

        isBig = true;
        anim.runtimeAnimatorController = bigMarioController != null ? bigMarioController : smallMarioController;

        inPowerUpSequence = false;

        // 启动倒计时（若已有计时器则重置）
        if (bigMarioDuration > 0f)
        {
            if (bigMarioTimerCoroutine != null)
                StopCoroutine(bigMarioTimerCoroutine);
            bigMarioTimerCoroutine = StartCoroutine(BigMarioTimer());
        }

        // --- ??????????? Center ????????????? ---
        // ?????????? Hs??????? Hb?????????????????????? Offset ???????
        // Offset = (Hb - Hs) / 2
        //if (isBig)
        //{
        //    col.size = new Vector2(0.8f, 1.2f);   // ??????????? 1.8
        //    col.offset = new Vector2(0f, 0.1f);  // (1.8 - 1.0) / 2 = 0.4
        //}
        //else
        //{
        //    col.size = new Vector2(0.8f, 1.0f);   // ?????????? 1.0
        //    col.offset = new Vector2(0f, 0f);    // ???????
        //}
    }

    private void ShootFireball()
    {
        // 【新增音效】播放发射火球声音
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.fireballClip);

        int dir = spriteRenderer.flipX ? -1 : 1;
        Vector3 spawnPos = new Vector3(transform.position.x + dir * 0.5f, transform.position.y, -2f);

        GameObject fb = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);
        fb.GetComponent<FireballController>()?.Init(dir);

        activeFireballs++;
        // 火球销毁时还原计数
        Destroy(fb, fb.GetComponent<FireballController>() != null
            ? fb.GetComponent<FireballController>().maxDistance / Mathf.Max(fb.GetComponent<FireballController>().speed, 0.1f) + 0.1f
            : 3f);
        StartCoroutine(TrackFireball(fb));
    }

    private System.Collections.IEnumerator TrackFireball(GameObject fb)
    {
        while (fb != null)
            yield return null;
        activeFireballs = Mathf.Max(0, activeFireballs - 1);
    }

    private IEnumerator BigMarioTimer()
    {
        yield return new WaitForSeconds(bigMarioDuration);
        ShrinkToSmall();
    }

    /// <summary> 吃到道具时调用，仅在小马里奥状态下触发变大流程 </summary>
    public void PowerUp()
    {
        if (isBig || inPowerUpSequence) return;
        StartCoroutine(PowerUpSequence());
    }

    /// <summary> 被怪物碰到或倒计时结束时调用，变回小马里奥 </summary>
    public void ShrinkToSmall()
    {
        if (!isBig) return;
        isBig = false;

        // 【新增音效】播放受伤缩小的声音
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.shrinkClip);

        if (bigMarioTimerCoroutine != null)
        {
            StopCoroutine(bigMarioTimerCoroutine);
            bigMarioTimerCoroutine = null;
        }

        if (anim != null && smallMarioController != null)
            anim.runtimeAnimatorController = smallMarioController;
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