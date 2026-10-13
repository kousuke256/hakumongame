
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField, Header("移動速度")]
    private float moveSpeed = 2f;

    [SerializeField, Header("画面外の移動許容距離")]
    private float cameraMargin = 1f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        if (rb == null || sr == null || mainCamera == null)
        {
            return;
        }

        // カメラの表示範囲を確認
        if (IsInsideCamera())
        {
            EnemyMove();
        }
        else
        {
            // 画面外では横移動を停止
            rb.linearVelocity = new Vector2(
                0f,
                rb.linearVelocity.y
            );
        }
    }

    bool IsInsideCamera()
    {
        float halfWidth =
            mainCamera.orthographicSize * mainCamera.aspect;

        float cameraX = mainCamera.transform.position.x;
        float enemyX = transform.position.x;

        float left = cameraX - halfWidth - cameraMargin;
        float right = cameraX + halfWidth + cameraMargin;

        return enemyX >= left && enemyX <= right;
    }

    private void EnemyMove()
    {
        float direction;

        if (sr.flipX == false)
        {
            direction = 1f;
        }
        else
        {
            direction = -1f;
        }

        rb.linearVelocity = new Vector2(
            -direction * moveSpeed,
            rb.linearVelocity.y
        );
    }
}
