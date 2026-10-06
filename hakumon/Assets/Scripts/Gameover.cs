using UnityEngine;
using UnityEngine.SceneManagement;

public class Gameover : MonoBehaviour
{
    public LifeManager lifeManager;
    private void OnTriggerEnter2D(Collider2D other)  //触れたかをチェック
    {
        if (other.CompareTag("Player"))  //相手がplayerか
        {
            lifeManager.LoseLife();
            SceneManager.LoadScene("GameOver");  //ここにscenename BuildProfileに追加してからやってね
        }
    }
}
