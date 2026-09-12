using Unity.Mathematics;
using UnityEngine;

public class Bullet2 : MonoBehaviour
{
    private Rigidbody2D rb;
    public float gravityPower = 10f;
    private float bulletDirecitonY;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.AddForce(Vector2.down * gravityPower * bulletDirecitonY);
    }

    public void Shoot(int bulletDirecitonX, float playerGravityDirection, float speed, bool canNanameShoot)
    {
        Debug.Log(speed);
        bulletDirecitonY = playerGravityDirection;
        if (canNanameShoot)
        {
            rb.linearVelocity = new Vector2(bulletDirecitonX * speed / 1.42f, bulletDirecitonY * speed / 1.42f);//1.42fはルート2の近似
        }
        else
        {
            rb.linearVelocity = new Vector2(bulletDirecitonX * speed , 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
