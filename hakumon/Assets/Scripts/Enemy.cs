using UnityEngine;

//カメラの子オブジェクトでTagがBackGroundのオブジェクトに重なっていない場合は動かないように修正
public class Enemy : MonoBehaviour
{
    [SerializeField,Header("移動速度")]
    private float moveSpeed;
    private bool canMove=false;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(canMove) EnemyMove();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // カメラ範囲に入ったら動く
        if (other.CompareTag("BackGround"))
        {
            canMove = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // カメラ範囲から出たら止まる
        if (other.CompareTag("BackGround"))
        {
            canMove = false;
        }
    }
    private void EnemyMove()
    {
        //移動方向を示す変数
        float direction;
        //キャラクターの向きで移動方向を設定
        //反転してないとき1,反転させて時-1に
        if(sr.flipX == false)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }
        rb.linearVelocity = new Vector2(-direction*moveSpeed,rb.linearVelocity.y);
    }
    
}
