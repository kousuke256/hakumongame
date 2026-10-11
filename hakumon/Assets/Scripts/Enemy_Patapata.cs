using UnityEngine;
public class Patapata : MonoBehaviour
{
    [SerializeField] private Rigidbody2D Enemy;  // 力を加える相手
    [SerializeField] private float jumpPower = 5f; // 加える力の強さ

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
          Enemy.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }
}