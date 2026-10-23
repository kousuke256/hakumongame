using System.Runtime.CompilerServices;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public LifeManager lifeManager;
    public GameObject goalCanvas;
    private bool showImage = false;
    private AudioClip clearSound;

    private void Awake()
    {

    }

    void Update()
    {
        if(!showImage)
        return;

        if(Keyboard.current.enterKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("StageSelect");  //ここにscenename BuildProfileに追加してからやってね
        }
    }
    private void OnTriggerEnter2D(Collider2D other)  //触れたかをチェック
    {
        if(!other.CompareTag("Player"))
        return;  //相手がplayerか

        showImage = true;
        Time.timeScale = 0f;
        goalCanvas.SetActive(true);
        
    }
}