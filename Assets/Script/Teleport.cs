using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform targetPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Please note that this code assumes the player has a tag "Player" assigned to it in the Unity Editor.
        if (other.CompareTag("Player"))
        {
            other.transform.position = targetPoint.position;
        }
    }
}
