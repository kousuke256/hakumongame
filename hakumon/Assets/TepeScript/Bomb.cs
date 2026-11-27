using TMPro;

using UnityEngine;

using UnityEngine.Tilemaps;

public class Bomb : MonoBehaviour

{

    [Header("プレイヤー")]

    public Player player;

    [Header("爆弾の数字")]

    [Min(0)] public int startCount = 2;

    public TMP_Text countText;

    [Header("爆発")]

    [Min(0f)] public float blastRadius = 2.5f;

    public Tilemap crackedWallMap;

    public GameObject explosionEffect; // なくても動く

    public AudioClip explosionSound;

    [Header("数字の色")]

    public Color normalColor = Color.cyan;
    public Color zeroColor = Color.red;

    private int count;

    private bool exploded;

    void Awake()

    {

        count = startCount;

        ShowCount();

    }

    void OnEnable()

    {

        if (player != null)

            player.OnGravityFlipped += OnFlip;

    }

    void OnDisable()

    {

        if (player != null)

            player.OnGravityFlipped -= OnFlip;

    }

    void OnFlip()

    {

        if (exploded) return;

        // 追加：重力が変わった瞬間、爆弾の中心が画面内にあるか確認

        Camera cam = Camera.main;

        if (cam == null) return;

        Vector3 pos = cam.WorldToViewportPoint(transform.position);

        if (pos.z <= 0f || pos.x < 0f || pos.x > 1f ||

                           pos.y < 0f || pos.y > 1f)

            return;

        // ここから下は今までどおり

        if (count == 0)

        {

            Explode();

            return;

        }

        count--;

        ShowCount();

    }


    void ShowCount()

    {

        if (countText == null) return;

        countText.text = count.ToString();

        countText.color = count == 0 ? zeroColor : normalColor;

    }

    void Explode()

    {

        exploded = true;

        if (crackedWallMap != null)

            BreakCrackedWalls();

        if (explosionEffect != null)

            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, Camera.main.transform.position);
        }

        Destroy(gameObject);

    }

    void BreakCrackedWalls()

    {

        Vector3 center = transform.position;

        Vector3 range = new Vector3(blastRadius, blastRadius, 0f);

        // 爆弾の周囲にあるマスだけ調べる

        Vector3Int min = crackedWallMap.WorldToCell(center - range);

        Vector3Int max = crackedWallMap.WorldToCell(center + range);

        for (int x = min.x; x <= max.x; x++)

        {

            for (int y = min.y; y <= max.y; y++)

            {

                Vector3Int cell = new Vector3Int(x, y, 0);

                if (!crackedWallMap.HasTile(cell)) continue;

                Vector3 tileCenter = crackedWallMap.GetCellCenterWorld(cell);

                if (Vector2.Distance(center, tileCenter) <= blastRadius)

                    crackedWallMap.SetTile(cell, null);

            }

        }

    }

    // Sceneビューで爆弾を選ぶと、壊せる範囲が見える

    void OnDrawGizmosSelected()

    {

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, blastRadius);

    }

}
