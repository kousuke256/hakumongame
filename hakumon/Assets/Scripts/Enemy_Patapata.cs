using UnityEngine;
public class Patapata : MonoBehaviour
{
    //上下移動の速度と移動距離
    public float speed = 2f;
    public float moveDistance =2f;

    private Rigidbody2D rb;
    private Vector2 startPosition;

    void Start()
    {
        //Rigidbody2Dを取得
        rb = GetComponent<Rigidbody2D>();
        //敵の初期位置を保存
        startPosition = rb.position;
    }

    void FixedUpdate()
    {
        //初期位置を中心に上下移動
        float y = startPosition.y + Mathf.Sin(Time.fixedTime * speed) * moveDistance;
        //y座標変更
        rb.MovePosition(new Vector2(startPosition.x,y));

    }
}