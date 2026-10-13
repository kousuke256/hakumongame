using UnityEngine;
using UnityEngine.Tilemaps;

public class OnOffBlock : MonoBehaviour
{
    public bool OnOff;
    public float ToumeiDo;
    public Player PlayerScript;
    public TileBase On;
    public TileBase Off;
    private float oldGravityDirectionY = -1f;
    private Tilemap tilemap;
    private Color usu;
    private Color koi;
    private TilemapCollider2D col;



    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        col = GetComponent<TilemapCollider2D>();

        // 元の色を保存する
        koi = tilemap.color;
        usu = tilemap.color;
    
        // 薄い色の透明度を設定
        usu.a = ToumeiDo;

        // 初期状態を設定する
        if (OnOff)
        {
            ChangeAllTiles(On);
            col.isTrigger = false;
            gameObject.layer = LayerMask.NameToLayer("Ground");
            gameObject.tag = "Ground";
            
        }
        else
        {
            ChangeAllTiles(Off);
            //tilemap.color = usu;
            col.isTrigger = true;
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
                //tilemap.color = koi;
                col.isTrigger = false;
                gameObject.layer = LayerMask.NameToLayer("Ground");
                gameObject.tag = "Ground";
                
            }
            else
            {
                ChangeAllTiles(Off);
                //tilemap.color = usu;
                col.isTrigger = true;
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
                //tilemap.color = usu;
                col.isTrigger = true;
                gameObject.layer = LayerMask.NameToLayer("Default");
                gameObject.tag = "Untagged";
                
            }
            else
            {
                ChangeAllTiles(On);
                //tilemap.color = koi;
                col.isTrigger = false;
                gameObject.layer = LayerMask.NameToLayer("Ground");
                gameObject.tag = "Ground";
                
            }
        }
    }
}
