using UnityEngine;

public class CoinCollect : MonoBehaviour
{
public int coinValue = 1;

private void OnTriggerEnter2D(Collider2D other)
{
if (!other.transform.root.CompareTag("Mario")) return;
int current = PlayerPrefs.GetInt("Score", 0);
PlayerPrefs.SetInt("Score", current + coinValue);

Destroy(gameObject);
}
}