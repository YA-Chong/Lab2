using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 挂在主角上，负责：踩头判定、踩头反弹、被敌人碰到受伤/死亡。
/// 敌人侧只认 MonsterController（踩头调用 Stomped，侧面碰到则主角受伤）。当前仅做普通小马里奥。
/// </summary>
public class MarioCombat : MonoBehaviour
{
    [Header("踩头")]
    [Tooltip("踩到敌人后向上的反弹速度，不宜过大否则会飞出屏幕")]
    public float stompBounceSpeed = 6f;
    [Tooltip("判定为“从上方踩到”时，接触法线 Y 需大于此值")]
    [Range(0.3f, 1f)]
    public float stompNormalYThreshold = 0.5f;

    [Header("受伤（当前为小马里奥：受伤即死亡）")]
    [Tooltip("死亡后多少秒重新加载当前场景，0 表示不自动重载")]
    public float deathReloadDelay = 2f;
    [Tooltip("死亡时显示的 UI（如「游戏结束」提示），不填则不显示")]
    public GameObject deathHintUI;

    private Rigidbody2D rb;
    private Animator anim;
    private MarioController marioController;
    private bool isDead;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        marioController = GetComponent<MarioController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        var monster = collision.gameObject.GetComponent<MonsterController>();
        if (monster == null) return;

        // 判定：是否从上方踩到（接触法线朝上 = 踩到敌人头顶）
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y >= stompNormalYThreshold)
            {
                monster.Stomped();
                StompBounce();
                return;
            }
        }

        // 从侧面或下面碰到 → 马里奥受伤
        TakeDamage();
    }

    private void StompBounce()
    {
        if (rb != null)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, stompBounceSpeed);
    }

    private void TakeDamage()
    {
        if (isDead) return;
        isDead = true;

        if (marioController != null)
            marioController.enabled = false;

        if (anim != null)
            anim.SetTrigger("Die");

        if (deathHintUI != null)
            deathHintUI.SetActive(true);

        if (deathReloadDelay > 0f)
            Invoke(nameof(ReloadCurrentScene), deathReloadDelay);
    }

    private void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
