using UnityEngine;
using UnityEngine.SceneManagement;
public class SavePoint : MonoBehaviour
{
    public Transform respawnPoint;
    private bool collected = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player")) return;
        LifeManager manager = LifeManager.Instance;
        if (manager == null) return;
        collected = true;
        string sceneName = SceneManager.GetActiveScene().name;
        // このセーブポイントを識別する名前
        string pointID = sceneName + ":" + GetObjectPath(transform);
        // 復活後、同じポイントを取り直したか確認
        bool alreadySaved =
            manager.hasSavePoint &&
            manager.savedSceneName == sceneName &&
            lastSavePointID == pointID;
        manager.hasSavePoint = true;
        manager.respawnPosition = respawnPoint != null
            ? respawnPoint.position
            : other.transform.position;
        manager.savedSceneName = sceneName;
        lastSavePointID = pointID;
        if (!alreadySaved)
        {
            PlaySaveSound();
        }
        Destroy(gameObject);
    }
    // シーンを読み直しても記憶を残す
    private static string lastSavePointID;
    private static string GetObjectPath(Transform target)
    {
        string path = target.name;
        while (target.parent != null)
        {
            target = target.parent;
            path = target.name + "/" + path;
        }
        return path;
    }
    private void PlaySaveSound()
    {
        AudioClip clip =
            Resources.Load<AudioClip>("Checkpoint_Space8bit");
        if (clip == null)
        {
            Debug.LogWarning(
                "ResourcesにCheckpoint_Space8bit.wavを入れてください！");
            return;
        }
        // セーブポイントが消えても音が途切れないようにする
        GameObject soundObject = new GameObject("SavePointSound");
        DontDestroyOnLoad(soundObject);
        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = 0.5f;
        source.clip = clip;
        source.Play();
        Destroy(soundObject, clip.length + 0.1f);
    }
    // ゲーム起動時に、シーン切り替えを監視する
[RuntimeInitializeOnLoadMethod(
   RuntimeInitializeLoadType.BeforeSceneLoad)]
private static void RegisterSceneReset()
    {
        lastSavePointID = null;
        // 二重登録を防ぐ
        SceneManager.sceneLoaded -= ResetSaveOnStageSelect;
        SceneManager.sceneLoaded += ResetSaveOnStageSelect;
    }
    // ステージ選択に戻ったら、中間の記録を消す
    private static void ResetSaveOnStageSelect(
       Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "StageSelect") return;
        lastSavePointID = null;
        LifeManager manager = LifeManager.Instance;
        if (manager == null) return;
        manager.hasSavePoint = false;
        manager.respawnPosition = Vector3.zero;
        manager.savedSceneName = "";
    }
}