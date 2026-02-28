using UnityEngine;

public class KoopaController : MonoBehaviour
{
    private bool isShell = false;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            BecomeShell();
    }

    public void BecomeShell()
    {
        if (isShell) return;
        isShell = true;

        var monster = GetComponent<MonsterController>();
        if (monster != null) monster.movementEnabled = false;

        anim.SetBool("isShell", true);
        if (spriteRenderer != null) spriteRenderer.flipX = false;
    }
}