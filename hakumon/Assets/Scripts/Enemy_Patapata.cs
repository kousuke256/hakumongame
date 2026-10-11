using UnityEngine;

public class Patapata : MonoBehaviour
{
    private Rigidbody2D enemyRb;

    [SerializeField] private float jumpPower = 30f;
    [SerializeField,Header("0だと即ジャンプ")] private float coolTime = 0.5f; // クールタイム（0なら即ジャンプ）

    private bool canDetect = false;
    private bool isGrounded = false;
    private float timer = 0f;

    void Start()
    {
        enemyRb = transform.parent.GetComponent<Rigidbody2D>();

        // 初期誤判定防止
        Invoke(nameof(EnableDetect), 0.1f);
    }

    void EnableDetect()
    {
        canDetect = true;
    }

    void FixedUpdate()
    {
        if (!canDetect) return;

        if (isGrounded)
        {
            timer += Time.fixedDeltaTime;

            // クールタイムが終わったらジャンプ
            if (timer >= coolTime)
            {
                enemyRb.linearVelocity = Vector2.zero; // 安定ジャンプのため速度リセット
                enemyRb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

                timer = 0f;
                isGrounded = false; // 空中に戻る
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!canDetect) return;

        if (other.CompareTag("Ground"))
        {
            isGrounded = true;
            timer = 0f; // 着地した瞬間からクールタイム計測開始
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}