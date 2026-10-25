using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public LifeManager lifeManager;
    public GameObject goalCanvas;
    public AudioSource bgmAudioSource; // ★追加：BGMのAudioSourceをアサインする変数
    private bool showImage = false;
    private AudioClip clearSound;
    // 同じシーンの別のゴールとの二重再生も防ぐ
    private static bool goalProcessing = false;

    private void Awake()
    {
        goalProcessing = false;
        clearSound = Resources.Load<AudioClip>("StageClearSound");
    }

    private void Update()
    {
        if (!showImage) return;

        bool enterPressed = Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame;
        bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        if (enterPressed || mousePressed)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("StageSelect");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (showImage || goalProcessing) return;

        if (goalCanvas == null)
        {
            Debug.LogError("Goal Canvasを設定してください！");
            return;
        }

        // 音を鳴らす前に二重実行を防ぐ
        showImage = true;
        goalProcessing = true;

        // ★追加：ゴール時にBGMを停止する
        StopBGM();

        PlayClearSound();
        Time.timeScale = 0f;
        goalCanvas.SetActive(true);
    }

    // ★追加：BGM停止用の処理
    private void StopBGM()
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Stop();
        }
        else
        {
            // Inspectorで未設定の場合の予備処理：
            // "BGM" タグがついたオブジェクトがあればそのAudioSourceを停止する
            GameObject bgmObj = GameObject.FindWithTag("BGM");
            if (bgmObj != null)
            {
                AudioSource source = bgmObj.GetComponent<AudioSource>();
                if (source != null)
                {
                    source.Stop();
                }
            }
        }
    }

    private void PlayClearSound()
    {
        if (clearSound == null)
        {
            Debug.LogWarning("Resources/StageClearSound が見つかりません！");
            return;
        }

        GameObject soundObject = new GameObject("ClearSound");
        DontDestroyOnLoad(soundObject);

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.clip = clearSound;
        
        source.spatialBlend = 0f;
        source.loop = false;
        source.Play();

        // 音用オブジェクト自身が再生後の削除を担当する
        ClearSoundCleanup cleanup = soundObject.AddComponent<ClearSoundCleanup>();
        cleanup.Begin(clearSound.length);
    }
}

// Goal.csの中に一緒に入れてOK
public class ClearSoundCleanup : MonoBehaviour
{
    public void Begin(float duration)
    {
        StartCoroutine(DeleteAfterPlayback(duration));
    }

    private IEnumerator DeleteAfterPlayback(float duration)
    {
        // ゲーム停止中でも待ち時間が進む
        yield return new WaitForSecondsRealtime(duration + 0.1f);
        Destroy(gameObject);
    }
}