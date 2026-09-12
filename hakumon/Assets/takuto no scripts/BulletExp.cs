using Unity.Mathematics;
using UnityEngine;

public class BulletExp : MonoBehaviour
{
    private Rigidbody2D rb;
    public float gravityPower = 10f;
    private Vector2 gravityDirection = Vector2.down;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.AddForce(gravityDirection * gravityPower);
    }

    public void Shoot(Vector2 playerGravityDirection, float speed)
    {
        gravityDirection = playerGravityDirection;
        Debug.Log(speed);
        rb.linearVelocity = new Vector2(speed / 1.45f, speed / 1.45f);//1.45fはルート2の近似
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
