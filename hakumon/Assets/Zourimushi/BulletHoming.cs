using UnityEngine;

public class BulletHoming : MonoBehaviour
{
    
    [Header("弾の設定")]
    public float speed = 5f;
    public float lifeTime = 8f;

    [Header("追尾の設定")]
    public float turnSpeed = 90f;
    private float timer = 0f;
    private Rigidbody2D rb;
    private float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;
    public Player playerScript;
    public Transform player;

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

        // lifeTime秒経過したら人生を終わらせる
        timer += Time.fixedDeltaTime;
        if(timer >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 currentDirection = rb.linearVelocity.normalized;
        Vector2 targetDirection = ((Vector2)player.position - rb.position).normalized;

        // 現在の進行方向から、目標方向へ少しずつ回転する
        Vector2 newDirection = Vector2.MoveTowards(currentDirection, targetDirection, turnSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime).normalized;
        rb.linearVelocity = newDirection * speed;

        // 地面にある程度ぶつかったら消す
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