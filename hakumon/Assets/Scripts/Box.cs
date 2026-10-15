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
        rb.gravityScale = 0f; // 重力の二重適用を防ぐためにオフ
        
        effector = GetComponent<PlatformEffector2D>();
        player = FindAnyObjectByType<Player>();
        col = GetComponent<Collider2D>();
        hirameki=player.gravityDirection.y;
    }

    void FixedUpdate()
    {
        // プレイヤーが触れていなければ横方向の速度をゼロにする
        if (!isTouchingPlayer)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        // rb.mass を掛けることで、Massの値に関わらず秒速9.8m/s²で加速します
        if (player != null)
        {
            if (player.gravityDirection.y==-1)
            {
                if (hirameki == -1&&hirameki==player.gravityDirection.y)
                {
                    hirameki=1;
                    rb.AddForce(player.gravityDirection * 2f, ForceMode2D.Impulse);
                }
                rb.AddForce(player.gravityDirection * 9.8f * rb.mass);
            }
            if (player.gravityDirection.y==1)
            {
                if (hirameki == 1&&hirameki==player.gravityDirection.y)
                {
                    hirameki=-1;
                    rb.AddForce(player.gravityDirection * 2f, ForceMode2D.Impulse);
                }
                rb.AddForce(player.gravityDirection * 9.8f * rb.mass);
            }
            
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