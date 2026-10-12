using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    public Transform player;

    [Header("弾の設定")]
    public float speed = 5f;

    [Header("追尾の設定")]
    public float turnSpeed = 90f;
    private Rigidbody2D rb;

    private float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

    public Player playerScript;

    void Start()
    {
        // Rigidbody2Dを取得する
        rb = GetComponent<Rigidbody2D>();

        // 最初はPlayerの方向へ弾を向ける
        if (player == null)
        return;

        Vector2 direction = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    void FixedUpdate()
    {
        // Playerが存在しなければ何もしない
        if (player == null)
        return;

        // 現在の弾の進行方向を取得する
        Vector2 currentDirection = rb.linearVelocity.normalized;

        // 弾からPlayerに向かう方向を取得する
        Vector2 targetDirection = ((Vector2)player.position - rb.position).normalized;

        // 現在の進行方向から、目標方向へ少しずつ回転する
        Vector2 newDirection = Vector2.MoveTowards(currentDirection, targetDirection, turnSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime).normalized;

        // 新しい方向へ移動する
        rb.linearVelocity = newDirection * speed;

        if(Physics2D.OverlapCircle(transform.position,groundCheckRadius,groundLayer))
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        return;


        // playerにあたったらplayerの重力を反転させて球を削除
        if(playerScript.gravityDirection.y == -1)
        {
            player.GetComponent<SpriteRenderer>().flipY = true;
        }
        else
        {
            player.GetComponent<SpriteRenderer>().flipY = false;
        }
        playerScript.gravityDirection.y *= -1;
        playerScript.gravitychange();
        playerScript.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0, 0);

        Destroy(gameObject);
    }
}