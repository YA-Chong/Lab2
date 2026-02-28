using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果碰到的是马里奥
        if (collision.CompareTag("Mario"))
        {
            // 1. 剥夺控制权：获取马里奥的移动脚本并禁用，让他无法再接收玩家的按键输入
            var controller = collision.GetComponent<MarioController>();
            if (controller != null)
            {
                controller.enabled = false;
            }

            // 2. 物理静止（可选但推荐）：把他的刚体速度清零，防止他带着之前的惯性继续乱飞
            var rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                // rb.gravityScale = 0f; // 如果你想让他掉出屏幕后直接“悬停”住，可以把重力也设为0
            }

            // 3. 呼叫 GameManager 宣布失败
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
        else
        {
            // 如果怪物或龟壳掉下去了，直接销毁它们，节省性能
            Destroy(collision.gameObject);
        }
    }
}