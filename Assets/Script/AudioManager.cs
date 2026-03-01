using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource bgmSource;

    [Header("BGM Clips")]
    public AudioClip mainThemeBGM;

    [Header("SFX Clips")]
    public AudioClip jumpClip;
    public AudioClip coinClip;
    public AudioClip stompClip;
    public AudioClip powerUpClip;
    public AudioClip shrinkClip;
    public AudioClip fireballClip;
    public AudioClip deathClip;

    // 【新增】胜利/过关音效
    [Tooltip("过关时播放的音效或音乐")]
    public AudioClip winClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (mainThemeBGM != null)
        {
            PlayBGM(mainThemeBGM);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayBGM(AudioClip bgmClip)
    {
        if (bgmSource == null || bgmClip == null) return;

        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }

    // 【新增】播放胜利音效的方法
    public void PlayWinSound()
    {
        // 1. 停止原有的 BGM
        StopBGM();

        // 2. 播放胜利音效 (通常胜利音效比较长，也可以用 bgmSource 来播，只要不循环就行)
        if (winClip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(winClip);
        }
    }
}