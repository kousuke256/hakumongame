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

    public void Shoot(int bulletDirecitonX, float playerGravityDirection, float speed, float theta)
    {
        //Debug.Log(angle);
        bulletDirecitonY = playerGravityDirection;
        rb.linearVelocity = new Vector2(bulletDirecitonX * speed * math.cos(theta), bulletDirecitonY * speed * math.sin(theta));
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ground"))
        {
            if (transform.position.y * bulletDirecitonY < 4.5f)
            {
                Destroy(gameObject);
            }
        }
    }
}
