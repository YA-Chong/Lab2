using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
public static HUDController Instance;

[Header("HUD Text")]
public TextMeshProUGUI marioScoreText; 
public TextMeshProUGUI coinText;
public TextMeshProUGUI worldText; 
public TextMeshProUGUI timeText;  

[Header("Values")]
public int score = 0;
public int coins = 0;
public int time = 400;
public string world = "1-1";

float timeAccumulator = 0f;

private void Awake()
{
if (Instance != null && Instance != this)
{
Destroy(gameObject);
return;
}
Instance = this;
}

private void Start()
{
RefreshAll();
}

private void Update()
{
timeAccumulator += Time.deltaTime;
if (timeAccumulator >= 1f)
{
timeAccumulator -= 1f;
if (time > 0) time--;
RefreshTime();
}
}

public void AddScore(int amount)
{
score += amount;
RefreshScore();
}

public void AddCoin(int amount = 1)
{
coins += amount;
RefreshCoins();
}

public void SetWorld(string newWorld)
{
world = newWorld;
RefreshWorld();
}

void RefreshAll()
{
RefreshScore();
RefreshCoins();
RefreshWorld();
RefreshTime();
    }

void RefreshScore()
{
if (marioScoreText != null)
marioScoreText.text = $"MARIO\n{score:D6}";
}

void RefreshCoins()
{
if (coinText != null)
coinText.text = $"COIN\nx{coins:D2}";
}

void RefreshWorld()
{
if (worldText != null)
worldText.text = $"WORLD\n{world}";
}

void RefreshTime()
{
if (timeText != null)
timeText.text = $"TIME\n{time:D3}";
}
}