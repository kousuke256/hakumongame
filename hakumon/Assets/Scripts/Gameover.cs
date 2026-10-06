using UnityEngine;
using UnityEngine.SceneManagement;
public class Gameover : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player以外が触れた場合は何もしない
        if (!other.CompareTag("Player"))
        {
            return;
        }
        // 本当に接触判定が発生しているか確認
        Debug.Log("Playerが敵にぶつかった");
        // LifeManagerが存在するか確認
        if (LifeManager.Instance == null)
        {
            Debug.LogError("LifeManagerが存在しません！");
            return;
        }
        // 残機を1減らす
        LifeManager.Instance.LoseLife();
        Debug.Log("現在の残機：" + LifeManager.Instance.lifeCount);
        // 残機が0以下ならGameOverへ
        if (LifeManager.Instance.lifeCount <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
        else
        {
            // まだ残機があるならZankiへ
            SceneManager.LoadScene("Zanki");
        }
    }
}