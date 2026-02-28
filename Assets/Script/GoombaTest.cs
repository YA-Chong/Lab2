using UnityEngine;

public class GoombaController : MonoBehaviour
{
    private bool isDead = false;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;
        // ????????? B ???????
        if (Input.GetKeyDown(KeyCode.B))
            Stomped();
    }

    public void Stomped()
    {
        if (isDead) return;
        isDead = true;

        var monster = GetComponent<MonsterController>();
        if (monster != null) monster.movementEnabled = false;

        anim.SetTrigger("Die");

        // ?????????????»…???????°§???????????
        GetComponent<Collider2D>().enabled = false;

        // ?????0.5???????????
        Destroy(gameObject, 0.5f);
    }
}