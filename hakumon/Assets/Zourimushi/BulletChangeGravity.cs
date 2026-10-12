using UnityEngine;

public class BulletChangeGravity : MonoBehaviour
{
    private float lifeTime = 8f;
    private float timer = 0f;
    private Rigidbody2D rb;
    private float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;
    public Player playerScript;
    public Transform player;

    void FixedUpdate()
    {
        // lifeTime秒経過したら人生を終わらせる
        timer += Time.fixedDeltaTime;
        if(timer >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }

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
        playerScript.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(playerScript.GetComponent<Rigidbody2D>().linearVelocity.x, 0);

        Destroy(gameObject);
    }

    public void Fire(Vector2 bulletDirection, float speed)
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = bulletDirection.normalized * speed;
    }
}