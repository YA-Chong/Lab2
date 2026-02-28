using UnityEngine;
using System.Collections;

public class MarioBrick : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float bounceHeight = 0.2f;
    public float bounceSpeed = 12f;

    [Header("Coin Settings")]
    public GameObject coinPrefab;
    public float coinSpawnHeight = 0.6f;

    [Header("Power-Up Settings")]
    [Tooltip("若设置了此项，顶砖块时弹出道具（花/蘑菇）而非金币")]
    public GameObject powerUpPrefab;

    [Header("Sprites")]
    public Sprite usedSprite;

    private bool isUsed = false;
    private Vector3 originalPosition;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        originalPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isUsed) return;

        if (collision.gameObject.CompareTag("Mario"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y > 0.5f)
                {
                    StartCoroutine(Bounce());
                    SpawnCoin();
                    isUsed = true;

                    if (usedSprite != null)
                        spriteRenderer.sprite = usedSprite;

                    break;
                }
            }
        }
    }

    void SpawnCoin()
    {
        // 若设置了道具 Prefab，弹道具；否则弹金币
        GameObject toSpawn = powerUpPrefab != null ? powerUpPrefab : coinPrefab;
        if (toSpawn != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * coinSpawnHeight;
            spawnPos.z = -2f;
            Instantiate(toSpawn, spawnPos, Quaternion.identity);
        }
    }

    IEnumerator Bounce()
    {
        Vector3 targetPosition = originalPosition + Vector3.up * bounceHeight;

        // Up
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                Time.deltaTime * bounceSpeed
            );
            yield return null;
        }

        // Down
        while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                originalPosition,
                Time.deltaTime * bounceSpeed
            );
            yield return null;
        }

        transform.position = originalPosition;
    }
}