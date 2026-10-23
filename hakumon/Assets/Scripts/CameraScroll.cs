using UnityEngine;
[RequireComponent(typeof(Camera))]
public class AutoScrollCamera : MonoBehaviour
{
    [Header("スクロール")]
    [SerializeField] private float scrollSpeed = 2f;
    [SerializeField] private float maxX = 500f;
    [Header("画面外の死亡判定")]
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private float outsideTiles = 2f;
    [SerializeField] private Transform player;
    private Camera cam;
    private BoxCollider2D deathArea;
    private void Start()
    {
        if (player != null)
        {
            Vector3 position = transform.position;
            position.x = player.position.x;
            transform.position = position;
        }
        cam = GetComponent<Camera>();
        // 左側の死亡エリアを自動作成
        GameObject area = new GameObject("ScrollDeathArea");
        area.transform.SetParent(transform, false);
        deathArea = area.AddComponent<BoxCollider2D>();
        deathArea.isTrigger = true;
        // 今の死亡処理・効果音をそのまま利用
        area.AddComponent<Gameover>();
        UpdateDeathArea();
    }
    private void LateUpdate()
    {
        // 右へ一定速度で移動
        Vector3 position = transform.position;
        position.x = Mathf.Min(
            position.x + Mathf.Max(0f, scrollSpeed) * Time.deltaTime,
            maxX
        );
        transform.position = position;
        UpdateDeathArea();
    }
    private void UpdateDeathArea()
    {
        if (deathArea == null)
            return;
        // カメラに映る範囲の横幅の半分
        float halfWidth = cam.orthographicSize * cam.aspect;
        // カメラの左端から、指定マス数だけ外側
        float deathBoundary =
            -halfWidth - tileSize * outsideTiles;
        // このエリアの右端が死亡境界になる
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