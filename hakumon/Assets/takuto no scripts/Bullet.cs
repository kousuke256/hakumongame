using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 3f;

    public void ShootBullet(Vector2 direction)
    {
        GetComponent<Rigidbody2D>().linearVelocity = direction.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
