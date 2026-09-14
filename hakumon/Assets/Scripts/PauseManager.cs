using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool isPaused = false;
    private static PauseManager instance;
    void Awake()
    {
        // PauseManagerのオブジェクトが既にあるなら削除する（pauseManagerが何個も作られないようにする対策）
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // sceneが切り替わってもオブジェクトを消さないメゾット
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // 現在のシーンがTitleならPauseManuを表示しない
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            return;
        }

        //escapeKeyが押されたらpauseMenuを開く
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePouse();
        }
    }

    void TogglePouse()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);

        //時間の流れを止める
        Time.timeScale = isPaused ? 0f : 1f;
    }
    //pauseMenuにボタンを追加するのならここに新しいメゾットを作って、ボタンを押したときにここのメゾットを実行させること

    public void ReturnToTitle()
    {
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenu.SetActive(false);
        SceneManager.LoadScene("Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
