using UnityEngine;
using UnityEngine.SceneManagement;

public class Patapata : MonoBehaviour
{
    private Rigidbody2D enemyRb;
    private Camera mainCamera;

    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float cameraMargin = 1f;

    [SerializeField] private float jumpCooldown = 0.2f;
    private float jumpCooldownTimer = 0f;

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

        if (moveSpeed < 0f)
        {
            direction = 1;
        }

        wasInsideCamera = IsInsideCamera();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.fixedDeltaTime;
        }

        bool insideCamera = IsInsideCamera();

        if (insideCamera)
        {
            enemyRb.linearVelocity = new Vector2(
                direction * Mathf.Abs(moveSpeed),
                enemyRb.linearVelocity.y
            );

            if (//!wasInsideCamera
                 isGrounded
                && Mathf.Abs(enemyRb.linearVelocity.y) < 0.05f
                && jumpCooldownTimer <= 0f)
            {
                Jump();
            }
        }
        else
        {
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
        jumpCooldownTimer = jumpCooldown;

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

        // プレイヤーと接触
        if (collision.gameObject.CompareTag("Player"))
        {
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

        // Ground以外は無視
        if (!collision.gameObject.CompareTag("Ground"))
        {
            return;
        }

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // 壁
            if (Mathf.Abs(contact.normal.x) > 0.9f)
            {
                direction *= -1;

                enemyRb.linearVelocity = new Vector2(
                    direction * Mathf.Abs(moveSpeed),
                    enemyRb.linearVelocity.y
                );

                return;
            }

            // 地面
            if (contact.normal.y > 0.5f && !isGrounded)
            {
                isGrounded = true;

                if (IsInsideCamera()
                    && jumpCooldownTimer <= 0f)
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