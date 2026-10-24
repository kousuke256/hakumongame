using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Camera))]
public class AutoScrollCamera : MonoBehaviour
{
    [Header("プレイヤー")]
    [SerializeField] private Transform player;
    [Header("スクロール")]
    [SerializeField] private float scrollSpeed = 2f;
    [SerializeField] private float maxX = 500f;
    [Header("画面外の死亡判定")]
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private float outsideTiles = 2f;
    private Camera cam;
    private BoxCollider2D deathArea;
    private void Start()
    {
        cam = GetComponent<Camera>();
        if (!cam.orthographic)
        {
            Debug.LogError("カメラをOrthographicに設定してください！");
            enabled = false;
            return;
        }
        // Playerが未設定ならタグで探す
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }
        LifeManager manager = LifeManager.Instance;
        Vector3 position = transform.position;
        // 現在のステージのセーブ地点から開始
        if (manager != null
 && manager.hasSavePoint
 && manager.savedSceneName ==
               SceneManager.GetActiveScene().name)
        {
            position.x = manager.respawnPosition.x;
        }
        else if (player != null)
        {
            // セーブがなければPlayerの位置から開始
            position.x = player.position.x;
        }
        // 高さと奥行きは変更しない
        transform.position = position;
        // カメラ左側に死亡エリアを作成
        GameObject area = new GameObject("ScrollDeathArea");
        area.transform.SetParent(transform, false);
        deathArea = area.AddComponent<BoxCollider2D>();
        deathArea.isTrigger = true;
        // 既存の死亡処理を使う
        area.AddComponent<Gameover>();
        UpdateDeathArea();
    }
    private void LateUpdate()
    {
        Vector3 position = transform.position;
        // Max Xに達するまで右へ進む
        // セーブ位置がMax Xを超えていても左へ戻さない
        if (position.x < maxX)
        {
            position.x = Mathf.Min(
                position.x
                + Mathf.Max(0f, scrollSpeed) * Time.deltaTime,
                maxX
            );
        }
        transform.position = position;
        UpdateDeathArea();
    }
    private void UpdateDeathArea()
    {
        if (deathArea == null)
            return;
        // カメラに映る横幅の半分
        float halfWidth =
            cam.orthographicSize * cam.aspect;
        // 画面左端から指定マス数だけ外側
        float margin =
            Mathf.Max(0f, tileSize)
            * Mathf.Max(0f, outsideTiles);
        float deathBoundary = -halfWidth - margin;
        const float areaWidth = 1000f;
        deathArea.transform.localPosition = new Vector3(
            deathBoundary - areaWidth / 2f,
            0f,
            -transform.position.z
        );
        deathArea.size = new Vector2(
            areaWidth,
            10000f
        );
    }
}