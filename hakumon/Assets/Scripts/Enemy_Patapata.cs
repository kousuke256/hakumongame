using UnityEngine;

public class Patapata : MonoBehaviour
{
    private Rigidbody2D enemyRb;

    [SerializeField] private float jumpPower = 6f;

    private bool canDetect = false;

    void Start()
    {
        enemyRb = transform.parent.GetComponent<Rigidbody2D>();

        // 初期誤判定を防ぐ
        Invoke(nameof(EnableDetect), 0.1f);
    }

    void EnableDetect()
    {
        canDetect = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!canDetect) return;

        if (other.CompareTag("Ground"))
        {
            // 前の速度を完全に消す（これが安定ジャンプの鍵）
            enemyRb.linearVelocity = Vector2.zero;

            // 毎回同じ高さのジャンプ
            enemyRb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }
}