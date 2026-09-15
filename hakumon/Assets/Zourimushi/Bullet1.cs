using System;
using UnityEngine;

public class Bullet1 : MonoBehaviour
{
    public void ShootBullet(float speed, Vector2 direction)
    {
        GetComponent<Rigidbody2D>().linearVelocity =  direction.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
