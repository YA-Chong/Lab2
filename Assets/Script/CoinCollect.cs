using UnityEngine;

public class CoinCollect : MonoBehaviour
{
    public int scoreValue = 200;
    public int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.transform.root.CompareTag("Mario")) return;

        HUDController.Instance.AddScore(scoreValue);
        HUDController.Instance.AddCoin(coinValue);

        // 【新增音效】播放金币声音 (在销毁之前调用)
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.coinClip);

        Destroy(gameObject);
    }
}