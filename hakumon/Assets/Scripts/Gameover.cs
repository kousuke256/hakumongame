using UnityEngine;
using UnityEngine.SceneManagement;

public class Gameover : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D other)  //触れたかをチェック
    {
        if(other.CompareTag("Player"))  //相手がplayerか
        {
            SceneManager.LoadScene("GameOver");  //ここにscenename BuildProfileに追加してからやってね
        }
    }
}
