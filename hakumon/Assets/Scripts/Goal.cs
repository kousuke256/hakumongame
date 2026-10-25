using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class Goal : MonoBehaviour
{
    public LifeManager lifeManager;
    public GameObject goalCanvas;
    private bool showImage = false;
    private AudioClip clearSound;
    // 同じシーンの別のゴールとの二重再生も防ぐ
    private static bool goalProcessing = false;
    private void Awake()
    {
        goalProcessing = false;
        clearSound =
            Resources.Load<AudioClip>("StageClearSound");
    }
    private void Update()
    {
        if (!showImage) return;
        bool enterPressed =
            Keyboard.current != null &&
            Keyboard.current.enterKey.wasPressedThisFrame;
        bool mousePressed =
            Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame;
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
        PlayClearSound();
        Time.timeScale = 0f;
        goalCanvas.SetActive(true);
    }
    private void PlayClearSound()
    {
        if (clearSound == null)
        {
            Debug.LogWarning(
                "Resources/StageClearSound が見つかりません！"
            );
            return;
        }
        GameObject soundObject = new GameObject("ClearSound");
        DontDestroyOnLoad(soundObject);
        AudioSource source =
            soundObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.clip = clearSound;
        
        source.spatialBlend = 0f;
        source.loop = false;
        source.Play();
        // 音用オブジェクト自身が再生後の削除を担当する
        ClearSoundCleanup cleanup =
            soundObject.AddComponent<ClearSoundCleanup>();
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