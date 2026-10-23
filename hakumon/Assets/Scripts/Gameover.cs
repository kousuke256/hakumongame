using UnityEngine;
using UnityEngine.SceneManagement;
public class Gameover : MonoBehaviour
{
    private AudioClip deathSound;
    private void Awake()
    {
        deathSound = Resources.Load<AudioClip>("DeathSound");
    }
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
        PlayDeathSound();
        LifeManager.Instance.LoseLife();
        Debug.Log("現在の残機：" + LifeManager.Instance.lifeCount);
        // 残機が0以下ならGameOverへ
        if (LifeManager.Instance.lifeCount <= 0)
        {
            LifeManager.Instance.hasSavePoint = false;
            SceneManager.LoadScene("GameOver");
        }
        else
        {
            // まだ残機があるなら現在のステージを最初から再開
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    private void PlayDeathSound()
    {
        if (deathSound == null)
            return;
        // 音を鳴らす専用オブジェクト
        GameObject soundObject = new GameObject("DeathSound");
        // シーンが変わっても音を残す
        DontDestroyOnLoad(soundObject);
        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.clip = deathSound;
        source.volume = 0.7f;
        source.spatialBlend = 0f;
        source.loop = false;
        source.Play();
        // 再生終了後に片付ける
        Destroy(soundObject, deathSound.length + 0.1f);
    }
}