using UnityEngine;
using UnityEngine.Tilemaps;

public class OnOffBlock : MonoBehaviour
{
    public bool OnOff;
    public Player PlayerScript;
    public TileBase On;
    public TileBase Off;
    private float oldGravityDirectionY = -1f;
    private Tilemap tilemap;
    private TilemapCollider2D col;
    private CompositeCollider2D compositeCol;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        col = GetComponent<TilemapCollider2D>();
        compositeCol = GetComponent<CompositeCollider2D>();


        // 初期状態を設定する
        if (OnOff)
        {
            ChangeAllTiles(On);
            col.isTrigger = false;
            compositeCol.isTrigger = false;
            gameObject.layer = LayerMask.NameToLayer("Ground");
            gameObject.tag = "Ground";
            
        }
        else
        {
            ChangeAllTiles(Off);
            col.isTrigger = true;
            compositeCol.isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("Default");
            gameObject.tag = "Untagged";
            
        }
    }

    public void ChangeAllTiles(TileBase image)
    {
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int position in bounds.allPositionsWithin)
        {
            // もともとタイルがある場所だけ変更する
            if (tilemap.HasTile(position))
            {
                tilemap.SetTile(position, image);
            }
        }
    }

    void Update()
    {
        if(oldGravityDirectionY == PlayerScript.gravityDirection.y)
        return;

        oldGravityDirectionY = PlayerScript.gravityDirection.y;

        if(oldGravityDirectionY == -1)
        {
            // 下向きの重力に切り替えたら
            if (OnOff)
            {
                ChangeAllTiles(On);
                col.isTrigger = false;
                compositeCol.isTrigger = false;
                gameObject.layer = LayerMask.NameToLayer("Ground");
                gameObject.tag = "Ground";
                
            }
            else
            {
                ChangeAllTiles(Off);
                col.isTrigger = true;
                compositeCol.isTrigger = true;
                gameObject.layer = LayerMask.NameToLayer("Default");
                gameObject.tag = "Untagged";
                
            }
        }
        else
        {
            // 上向きの重力に切り替えたら
            if (OnOff)
            {
                ChangeAllTiles(Off);
                col.isTrigger = true;
                compositeCol.isTrigger = true;
                gameObject.layer = LayerMask.NameToLayer("Default");
                gameObject.tag = "Untagged";
                
            }
            else
            {
                ChangeAllTiles(On);
                col.isTrigger = false;
                compositeCol.isTrigger = false;
                gameObject.layer = LayerMask.NameToLayer("Ground");
                gameObject.tag = "Ground";
                
            }
        }
    }
}
