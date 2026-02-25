using UnityEngine;

public class KoopaController : MonoBehaviour
{
    public float speed = 2f;
    private int direction = -1;
    private bool isShell = false;

    private SpriteRenderer spriteRenderer;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!isShell)
        {
            // 正常行走
            transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

            // 翻转图片：向左走时 flipX 为 true
            spriteRenderer.flipX = (direction == -1);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            BecomeShell();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        direction *= -1;
    }

    void BecomeShell()
    {
        isShell = true;
        anim.SetBool("isShell", true);

        // 缩壳后通常不再左右翻转，保持静止
        spriteRenderer.flipX = false;
    }
}