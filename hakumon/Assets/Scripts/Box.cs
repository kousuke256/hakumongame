using UnityEngine;

public class Box : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    [SerializeField] private float maxHorizontalSpeed = 5f;      // 横移動の最大速度
    [SerializeField] private float horizontalAcceleration = 20f; // 横移動の加速度

    private Rigidbody2D rb;
    private bool isTouchingPlayer;

    private PlatformEffector2D effector;
    private Player player;
    private Collider2D col;

    private float hirameki;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // Unityの重力は使わない

        effector = GetComponent<PlatformEffector2D>();
        player = FindAnyObjectByType<Player>();
        col = GetComponent<Collider2D>();

        if (player != null)
        {
            hirameki = player.gravityDirection.y;
        }
    }

    void FixedUpdate()
    {
        if (player == null)
            return;

        // --- 横移動の速度制限と加速制御 ---
        float newXVelocity = 0f;

        if (isTouchingPlayer)
        {
            // プレイヤー接触時：物理で付与された横速度を最大速度内にクランプして加速
            float targetXSpeed = Mathf.Clamp(rb.linearVelocity.x, -maxHorizontalSpeed, maxHorizontalSpeed);
            newXVelocity = Mathf.MoveTowards(
                rb.linearVelocity.x,
                targetXSpeed,
                horizontalAcceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            // プレイヤー非接触時：即座に横速度をゼロにする
            newXVelocity = 0f;
        }

        rb.linearVelocity = new Vector2(newXVelocity, rb.linearVelocity.y);


        // ▼ 重力方向が下向きの場合
        if (player.gravityDirection.y == -1)
        {
            // 重力反転した瞬間に一度だけImpulseを加える処理
            if (hirameki == -1 && hirameki == player.gravityDirection.y)
            {
                hirameki = 1;
                rb.AddForce(player.gravityDirection * 2f * rb.mass, ForceMode2D.Impulse);
            }

            // 毎フレーム重力加速度を加える
            rb.AddForce(player.gravityDirection * 9.8f * rb.mass);
        }

        // ▼ 重力方向が上向きの場合
        if (player.gravityDirection.y == 1)
        {
            // 重力反転した瞬間に一度だけImpulseを加える処理
            if (hirameki == 1 && hirameki == player.gravityDirection.y)
            {
                hirameki = -1;
                rb.AddForce(player.gravityDirection * 2f * rb.mass, ForceMode2D.Impulse);
            }

            // 毎フレーム重力加速度を加える
            rb.AddForce(player.gravityDirection * 9.8f * rb.mass);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
            // 離れた瞬間に物理計算上の横速度も0にする
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }
}