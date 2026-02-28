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
        if (coinPrefab != null)
        {
            Instantiate(
                coinPrefab,
                transform.position + Vector3.up * coinSpawnHeight,
                Quaternion.identity
            );
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