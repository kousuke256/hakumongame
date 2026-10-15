
using UnityEngine;
using UnityEngine.Tilemaps;

public class OnOffBlock : MonoBehaviour
{
    public bool OnOff;
    public Player PlayerScript;
    public TileBase On;
    public TileBase Off;

    private Tilemap tilemap;
    private TilemapCollider2D tileCollider;
    private CompositeCollider2D compositeCollider;

    private float oldGravityDirectionY;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        tileCollider = GetComponent<TilemapCollider2D>();
        compositeCollider = GetComponent<CompositeCollider2D>();

        // Playerが未設定なら自動取得
        if (PlayerScript == null)
        {
            PlayerScript = FindFirstObjectByType<Player>();
        }

        if (PlayerScript == null)
        {
            Debug.LogError(
                "OnOffBlock: Playerが見つかりません！",
                this
            );
            enabled = false;
            return;
        }

        if (tilemap == null)
        {
            Debug.LogError(
                "OnOffBlock: Tilemapがありません！",
                this
            );
            enabled = false;
            return;
        }

        // 開始時の重力方向を取得
        oldGravityDirectionY = PlayerScript.gravityDirection.y;

        
        UpdateBlock();
    }

    void Update()
    {
        if (PlayerScript == null) return;

        float currentGravityY =
            PlayerScript.gravityDirection.y;

        // 重力方向が変わっていなければ何もしない
        if (Mathf.Approximately(
            oldGravityDirectionY,
            currentGravityY
        ))
        {
            return;
        }

        oldGravityDirectionY = currentGravityY;

        // 重力反転時に切り替える
        UpdateBlock();
    }

    void UpdateBlock()
    {
        bool reverseGravity =
            PlayerScript.gravityDirection.y > 0f;

        bool active = reverseGravity ? !OnOff : OnOff;

        if (active)
        {
            ChangeAllTiles(On);

            if (tileCollider != null)
                tileCollider.isTrigger = false;

            if (compositeCollider != null)
                compositeCollider.isTrigger = false;

            gameObject.layer =
                LayerMask.NameToLayer("Ground");

            gameObject.tag = "Ground";
        }
        else
        {
            ChangeAllTiles(Off);

            if (tileCollider != null)
                tileCollider.isTrigger = true;

            if (compositeCollider != null)
                compositeCollider.isTrigger = true;

            gameObject.layer =
                LayerMask.NameToLayer("Default");

            gameObject.tag = "Untagged";
        }
    }

    public void ChangeAllTiles(TileBase image)
    {
        if (tilemap == null || image == null) return;

        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int position in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(position))
            {
                tilemap.SetTile(position, image);
            }
        }
    }
}
