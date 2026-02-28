using UnityEngine;

/// <summary>
/// 挂在花/蘑菇等道具 Prefab 上（Collider2D 勾 Is Trigger）。
/// 马里奥碰到后触发变大，加分，销毁自身。
/// </summary>
public class PowerUpCollect : MonoBehaviour
{
    [Tooltip("吃到道具时增加的分数")]
    public int scoreValue = 1000;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.transform.root.CompareTag("Mario")) return;

        var mario = other.transform.root.GetComponent<MarioController>();
        if (mario != null)
            mario.PowerUp();

        if (HUDController.Instance != null)
            HUDController.Instance.AddScore(scoreValue);

        Destroy(gameObject);
    }
}
