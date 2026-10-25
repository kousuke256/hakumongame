using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Goal1 : MonoBehaviour
{
    public LifeManager lifeManager;
    public GameObject goalCanvas;
    public AudioSource bgmAudioSource;
    private bool showImage = false;
    private AudioClip clearSound;
    private static bool goalProcessing = false;

    private void Awake()
    {
        goalProcessing = false;
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
            SceneManager.LoadScene("Endroll");
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

        showImage = true;
        goalProcessing = true;

        StopBGM();
        PlayClearSound();

        Time.timeScale = 0f;
        goalCanvas.SetActive(true);
    }

    private void StopBGM()
    {
        if (bgmAudioSource != null)
        {
            bgmAudioSource.Stop();
        }
        else
        {
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

        // Goal.cs 側で定義されている ClearSoundCleanup をそのまま自動で参照して使います
        ClearSoundCleanup cleanup = soundObject.AddComponent<ClearSoundCleanup>();
        cleanup.Begin(clearSound.length);
    }
}