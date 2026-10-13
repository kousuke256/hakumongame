
using UnityEngine;
using UnityEngine.SceneManagement;

public class Patapata : MonoBehaviour
{
    private Rigidbody2D enemyRb;
    private Camera mainCamera;

    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float cameraMargin = 1f;

    // -1：左、1：右
    private int direction = -1;

    private bool isGrounded = false;
    private bool isDead = false;
    private bool wasInsideCamera = false;

    void Start()
    {
        enemyRb = GetComponentInParent<Rigidbody2D>();
        mainCamera = Camera.main;

        if (enemyRb == null || mainCamera == null)
        {
            Debug.LogError("Rigidbody2DまたはMainCameraがありません！");
            enabled = false;
            return;
        }

        // Move Speedが負の場合は右向きで開始
        if (moveSpeed < 0f)
        {
            direction = 1;
        }

        wasInsideCamera = IsInsideCamera();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        bool insideCamera = IsInsideCamera();

        if (insideCamera)
        {
            // 現在の進行方向に移動
            enemyRb.linearVelocity = new Vector2(
                direction * Mathf.Abs(moveSpeed),
                enemyRb.linearVelocity.y
            );

            if (!wasInsideCamera && isGrounded)
            {
                Jump();
            }
        }
        else
        {
            // カメラ外では横移動停止
            enemyRb.linearVelocity = new Vector2(
                0f,
                enemyRb.linearVelocity.y
            );
        }

        wasInsideCamera = insideCamera;
    }

    bool IsInsideCamera()
    {
        float halfWidth =
            mainCamera.orthographicSize * mainCamera.aspect;

        float cameraX = mainCamera.transform.position.x;
        float enemyX = enemyRb.position.x;

        float left = cameraX - halfWidth - cameraMargin;
        float right = cameraX + halfWidth + cameraMargin;

        return enemyX >= left && enemyX <= right;
    }

    void Jump()
    {
        enemyRb.linearVelocity = new Vector2(
            enemyRb.linearVelocity.x,
            0f
        );

        enemyRb.AddForce(
            Vector2.up * jumpPower,
            ForceMode2D.Impulse
        );
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        // プレイヤーとの接触
        if (collision.gameObject.CompareTag("Player"))
        {
            // 上から踏まれた場合
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f)
                {
                    return;
                }
            }

            if (LifeManager.Instance == null)
            {
                Debug.LogError("LifeManagerがありません！");
                return;
            }

            isDead = true;
            LifeManager.Instance.LoseLife();

            if (LifeManager.Instance.lifeCount <= 0)
            {
                SceneManager.LoadScene("GameOver");
            }
            else
            {
                SceneManager.LoadScene(
                    SceneManager.GetActiveScene().buildIndex
                );
            }

            return;
        }

        // 壁との接触
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // 横方向から壁に当たった場合
                if (Mathf.Abs(contact.normal.x) > 0.5f)
                {
                    direction *= -1;

                    enemyRb.linearVelocity = new Vector2(
                        direction * Mathf.Abs(moveSpeed),
                        enemyRb.linearVelocity.y
                    );

                    return;
                }
            }
        }

        // 地面との接触
        if (!collision.gameObject.CompareTag("Ground"))
        {
            return;
        }

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f && !isGrounded)
            {
                isGrounded = true;

                if (IsInsideCamera())
                {
                    Jump();
                }

                return;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
