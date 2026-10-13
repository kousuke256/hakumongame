using UnityEngine;
using UnityEngine.Tilemaps;

public class OnOffBlock : MonoBehaviour
{
    public bool OnOff;
    public float ToumeiDo;
    public Player PlayerScript;
    private float oldGravityDirectionY = -1f;
    private Tilemap tilemap;
    private Color usu;
    private Color koi;
    private TilemapCollider2D col;
        


    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        col = GetComponent<TilemapCollider2D>();

        if (OnOff)
        {
            tilemap.color = Color.green;
        }
        else
        {
            tilemap.color = Color.blue;
        }

        // 元の色を保存する
        koi = tilemap.color;
        usu = tilemap.color;
    
        // 薄い色の透明度を設定
        usu.a = ToumeiDo;

        // 初期状態を設定する
        if (OnOff)
        {
            col.isTrigger = false;
            gameObject.layer = LayerMask.NameToLayer("Ground");
            gameObject.tag = "Ground";
        }
        else
        {
            tilemap.color = usu;
            col.isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("Default");
            gameObject.tag = "Default";
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
                tilemap.color = koi;
                col.isTrigger = false;
                gameObject.layer = LayerMask.NameToLayer("Ground");
                gameObject.tag = "Ground";
            }
            else
            {
                tilemap.color = usu;
                col.isTrigger = true;
                gameObject.layer = LayerMask.NameToLayer("Default");
                gameObject.tag = "Default";
            }
        

        }
        else
        {
            // 上向きの重力に切り替えたら
            if (OnOff)
            {
                tilemap.color = usu;
                col.isTrigger = true;
                gameObject.layer = LayerMask.NameToLayer("Default");
                gameObject.tag = "Default";
            }
            else
            {
                tilemap.color = koi;
                col.isTrigger = false;
                gameObject.layer = LayerMask.NameToLayer("Ground");
                gameObject.tag = "Ground";
            }
        }
    }
}
