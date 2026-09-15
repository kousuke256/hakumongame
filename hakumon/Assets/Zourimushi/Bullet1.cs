using System;
using UnityEngine;

public class Bullet1 : MonoBehaviour
{
    SpriteRenderer sr;
    public void ShootBullet(float speed, Vector2 direction, int a)
    {
        if (a ==1)
        {
            sr = GetComponent<SpriteRenderer>();
            sr.color = Color.green; 
        }
        if (a ==2)
        {
            sr = GetComponent<SpriteRenderer>();
            sr.color = Color.red; 
        }
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
