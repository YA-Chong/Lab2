using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 单例模式：让其他脚本可以直接通过 GameManager.Instance 调用这里的方法
    public static GameManager Instance { get; private set; }

    [Header("UI 设置")]
    public GameObject winUI;
    public GameObject loseUI;

    [Header("游戏设置")]
    public float reloadDelay = 2f;

    // 记录游戏是否已经结束，防止多次触发（比如同时碰到怪物和掉下悬崖）
    private bool isGameOver = false;

    private void Awake()
    {
        // 单例初始化
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 【新增】每次关卡重新加载（或复活）时，通知老员工 AudioManager 重新播放 BGM
        if (AudioManager.Instance != null && AudioManager.Instance.mainThemeBGM != null)
        {
            AudioManager.Instance.PlayBGM(AudioManager.Instance.mainThemeBGM);
        }
    }

    /// <summary> 触发胜利（到达终点） </summary>
    public void GameWin()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (winUI != null) winUI.SetActive(true);
        Debug.Log("玩家胜利！");
        // 这里可以写加载下一关的逻辑，比如 Invoke(nameof(LoadNextLevel), reloadDelay);
    }

    /// <summary> 触发失败（碰到怪物死亡 或 掉出地图） </summary>
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (loseUI != null) loseUI.SetActive(true);
        Debug.Log("游戏结束！");

        // 延迟重载当前关卡
        if (reloadDelay > 0f) Invoke(nameof(ReloadScene), reloadDelay);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}