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

    // 弾の初期化が終わったかどうか
    private bool isInitialized = false;

    void Awake()
    {
        // Rigidbody2Dを取得する
        rb = GetComponent<Rigidbody2D>();
    }

    // 弾を生成した側から呼び出す初期化メソッド
    public void Initialize(Transform target)
    {
        // 追尾するPlayerを設定する
        player = target;

        // Playerのスクリプトを取得する
        playerScript = player.GetComponent<Player>();

        // Playerが見つからなければ処理を終了する
        if (player == null || playerScript == null)
        {
            Debug.LogError("HomingBullet: Playerの設定に失敗しました！");
            return;
        }

        // 最初はPlayerの方向へ弾を飛ばす
        Vector2 direction =
            ((Vector2)player.position - rb.position).normalized;

        rb.linearVelocity = direction * speed;

        // 初期化完了
        isInitialized = true;
    }

    void FixedUpdate()
    {
        // 初期化が終わっていなければ何もしない
        if (!isInitialized || player == null)
        {
            return;
        }

        Debug.Log(
        $"弾の位置：{bulletPosition}, " +
        $"Playerの位置：{playerPosition}, " +
        $"目標方向：{targetDirection}, " +
        $"現在の速度：{rb.linearVelocity}"
    );
        // 現在の弾の進行方向を取得する
        Vector2 currentDirection = rb.linearVelocity.normalized;

        // 弾からPlayerに向かう方向を取得する
        Vector2 targetDirection =
            ((Vector2)player.position - rb.position).normalized;

        // 少しずつPlayerの方向へ向きを変える
        Vector2 newDirection = Vector2.MoveTowards(
            currentDirection,
            targetDirection,
            turnSpeed * Mathf.Deg2Rad * Time.fixedDeltaTime
        ).normalized;

        // 新しい方向へ移動する
        rb.linearVelocity = newDirection * speed;

        // 地面に当たったら弾を削除する
        if (Physics2D.OverlapCircle(
            transform.position,
            groundCheckRadius,
            groundLayer))
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Player以外に当たった場合は何もしない
        if (!collision.CompareTag("Player"))
        {
            return;
        }

        // 初期化されていなければ何もしない
        if (!isInitialized || playerScript == null)
        {
            return;
        }

        // Playerの重力方向を反転させる
        playerScript.gravityDirection.y *= -1;

        // 重力方向に合わせて画像を反転させる
        SpriteRenderer sprite = player.GetComponent<SpriteRenderer>();

        if (sprite != null)
        {
            sprite.flipY = playerScript.gravityDirection.y == 1;
        }

        // Playerの重力を更新する
        playerScript.gravitychange();

        // Playerの速度をリセットする
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }

        // 弾を削除する
        Destroy(gameObject);
    }
}