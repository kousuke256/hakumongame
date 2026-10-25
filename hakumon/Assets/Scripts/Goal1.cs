using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Goal1 : MonoBehaviour
{
    public LifeManager lifeManager;
    public GameObject goalCanvas;
    public AudioSource bgmAudioSource; // ★追加：BGMのAudioSourceをアサインする変数
    private bool showImage = false;
    private AudioClip clearSound;
    private static bool goalProcessing = false; // ★追加：二重処理防止フラグ

    private void Awake()
    {
        goalProcessing = false;
        // Resourcesフォルダからクリア音を読み込む（Resources/StageClearSound.wav など）
        clearSound = Resources.Load<AudioClip>("StageClearSound");
    }

    void Update()
    {
        if (!showImage) return;

        bool enterPressed = Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame;
        bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;

        if (enterPressed || mousePressed)
        {
            ShowPanelOnReturn.showPanel = true;
            Time.timeScale = 1f;
            SceneManager.LoadScene("Endroll"); // ここにscenename BuildProfileに追加してからやってね
        }
    }

    private void OnTriggerEnter2D(Collider2D other) // 触れたかをチェック
    {
        if (!other.CompareTag("Player")) return; // 相手がplayerか
        if (showImage || goalProcessing) return;

        if (goalCanvas == null)
        {
            Debug.LogError("Goal Canvasを設定してください！");
            return;
        }

        showImage = true;
        goalProcessing = true;

        // ★追加：BGMを停止
        StopBGM();

        // ★追加：クリア音を再生
        PlayClearSound();

        Time.timeScale = 0f;
        goalCanvas.SetActive(true);
    }

    // ★追加：BGM停止処理
    private void StopBGM()
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Stop();
        }
        else
        {
            // Inspectorで未設定の場合、"BGM" タグがついたオブジェクトから自動検索して停止
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

    // ★追加：クリア音再生処理
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

        // ポーズ中（Time.timeScale = 0）でも音が消えないよう監視して削除
        ClearSoundCleanup cleanup = soundObject.AddComponent<ClearSoundCleanup>();
        cleanup.Begin(clearSound.length);
    }
}

// クリア音再生用オブジェクトの自動削除クラス（Goal1.csの末尾に一緒に入れてOK）
public class ClearSoundCleanup : MonoBehaviour
{
    public void Begin(float duration)
    {
        StartCoroutine(DeleteAfterPlayback(duration));
    }

    private IEnumerator DeleteAfterPlayback(float duration)
    {
        // Time.timeScale = 0 でポーズされていても時間が進む処理
        yield return new WaitForSecondsRealtime(duration + 0.1f);
        Destroy(gameObject);
    }
}