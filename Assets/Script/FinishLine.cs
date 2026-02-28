using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果碰到了标签是 "Mario" 的物体
        if (collision.CompareTag("Mario"))
        {
            // 呼叫 GameManager 宣布胜利
            GameManager.Instance.GameWin();

            // （可选）在这里可以获取 MarioController 并禁用它的控制，让马里奥自动走进城堡
        }
    }
}