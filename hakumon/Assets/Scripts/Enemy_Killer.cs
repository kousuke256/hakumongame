
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy_Killer : MonoBehaviour
{
    public enum MoveType
    {
        Straight,
        Homing
    }

    [Header("移動設定")]
    [SerializeField] private MoveType moveType = MoveType.Straight;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private bool moveLeft = true;

    [Header("追尾設定")]
    [SerializeField] private float searchRange = 10f;

    [Header("カメラ設定")]
    [SerializeField] private float cameraMargin = 1f;

    [Header("踏みつけ設定")]
    [SerializeField] private float bouncePower = 8f;
    [SerializeField, Range(0.1f, 0.5f)]
    private float stompArea = 0.3f;

    private Rigidbody2D rb;
    private Collider2D enemyCollider;
    private Camera mainCamera;
    private Transform playerTarget;

    private bool isDead = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Start()
    {
        mainCamera = Camera.main;

        GameObject target =
            GameObject.FindGameObjectWithTag("Player");

        if (target != null)
        {
            playerTarget = target.transform;
        }
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (!IsInsideCamera())
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = Vector2.zero;

        if (moveType == MoveType.Straight)
        {
            direction = moveLeft
                ? Vector2.left
                : Vector2.right;
        }
        else if (playerTarget != null)
        {
            Vector2 difference =
                (Vector2)playerTarget.position - rb.position;

            if (difference.magnitude <= searchRange)
            {
                direction = difference.normalized;
            }
        }

        rb.linearVelocity =
            direction * Mathf.Abs(moveSpeed);
    }

    bool IsInsideCamera()
    {
        if (mainCamera == null) return true;

        Vector3 viewport =
            mainCamera.WorldToViewportPoint(transform.position);

        if (viewport.z < 0f) return false;

        float halfWidth =
            mainCamera.orthographicSize * mainCamera.aspect;

        float marginX =
            cameraMargin / (halfWidth * 2f);

        float marginY =
            cameraMargin / (mainCamera.orthographicSize * 2f);

        return viewport.x >= -marginX &&
               viewport.x <= 1f + marginX &&
               viewport.y >= -marginY &&
               viewport.y <= 1f + marginY;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            CheckPlayerCollision(collision);
            return;
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }

    void CheckPlayerCollision(Collision2D collision)
    {
        Player player =
            collision.gameObject.GetComponent<Player>();

        Rigidbody2D playerRb =
            collision.gameObject.GetComponent<Rigidbody2D>();

        if (player == null || playerRb == null)
        {
            Debug.LogError("PlayerまたはRigidbody2Dがありません！");
            return;
        }

        // Player.csで管理している重力方向を取得
        Vector2 gravity = player.gravityDirection.normalized;

        if (gravity.sqrMagnitude < 0.01f)
        {
            gravity = Vector2.down;
        }

        bool reverseGravity = gravity.y > 0f;

        // キラーの当たり判定の範囲
        Bounds bounds = enemyCollider.bounds;

        float top = bounds.max.y;
        float bottom = bounds.min.y;
        float height = bounds.size.y;

        // プレイヤーとキラーの上下関係
        float playerY = playerRb.position.y;
        float killerY = bounds.center.y;

        bool correctSide = reverseGravity
            ? playerY < killerY
            : playerY > killerY;

        // 接触位置がキラーの上面・下面にあるか確認
        bool correctContact = false;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (!reverseGravity)
            {
                // 通常重力：キラーの上面
                if (contact.point.y >= top - height * stompArea)
                {
                    correctContact = true;
                    break;
                }
            }
            else
            {
                // 反転重力：キラーの下面
                if (contact.point.y <= bottom + height * stompArea)
                {
                    correctContact = true;
                    break;
                }
            }
        }

        if (correctSide && correctContact)
        {
            StompEnemy(playerRb, gravity);
        }
        else
        {
            DamagePlayer();
        }
    }

    void StompEnemy(
        Rigidbody2D playerRb,
        Vector2 gravity
    )
    {
        if (isDead) return;

        isDead = true;

        // 重力方向の速度を取り除く
        Vector2 velocity = playerRb.linearVelocity;

        velocity -= gravity *
            Vector2.Dot(velocity, gravity);

        // 重力と逆方向へ跳ね返る
        velocity -= gravity * bouncePower;

        playerRb.linearVelocity = velocity;

        // キラーを消滅
        Destroy(gameObject);
    }

    void DamagePlayer()
    {
        if (isDead) return;

        if (LifeManager.Instance == null)
        {
            Debug.LogError("LifeManagerがありません！");
            return;
        }

        isDead = true;
        rb.linearVelocity = Vector2.zero;

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
    }
}
