using UnityEngine;

/// <summary>
/// 怪物被踩死时的加分。挂在敌人 Prefab 上，在 Inspector 里配置分数；与 HUDController 配合（和吃金币共用同一套分数显示）。
/// 乌龟壳撞死敌人等逻辑后续可再扩展。
/// </summary>
public class MonsterStompScore : MonoBehaviour
{
    [Tooltip("踩死该怪物时增加的分数")]
    public int stompScore = 100;

    /// <summary> 被踩时由 MonsterController.Stomped() 调用 </summary>
    public void GrantStompScore()
    {
        if (HUDController.Instance == null) return;
        HUDController.Instance.AddScore(stompScore);
    }
}
