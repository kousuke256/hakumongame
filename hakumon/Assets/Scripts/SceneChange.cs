using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SeceneChange : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)  //触れたかをチェック
    {
        if(other.CompareTag("Player"))  //相手がplayerか
        {
            SceneManager.LoadScene("Goal");  //ここにscenename BuildProfileに追加してからやってね
        }
    }
}