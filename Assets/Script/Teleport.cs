using Unity.Cinemachine;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform targetPoint;
    public CinemachineCamera virtualCamera; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Mario"))
        {
            // 计算位移差值
            Vector3 delta = targetPoint.position - other.transform.position;

            // 玩家瞬移
            other.transform.position = targetPoint.position;

            // 通知Cinemachine这是一次瞬移
            virtualCamera.OnTargetObjectWarped(other.transform, delta);
        }
    }
}
