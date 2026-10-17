using UnityEngine;

public class Box : MonoBehaviour
{
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

        hirameki = player.gravityDirection.y;
    }

    void FixedUpdate()
    {
        // プレイヤーに触れていないときは横速度をゼロにする
        if (!isTouchingPlayer)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        if (player == null)
            return;

        // ▼ 重力方向が下向きの場合
        if (player.gravityDirection.y == -1)
        {
            // 重力反転した瞬間に一度だけImpulseを加える処理
            if (hirameki == -1 && hirameki == player.gravityDirection.y)
            {
                hirameki = 1;
                rb.AddForce(player.gravityDirection * 2f*rb.mass, ForceMode2D.Impulse);
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
                rb.AddForce(player.gravityDirection * 2f*rb.mass, ForceMode2D.Impulse);
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
        }
    }
}
