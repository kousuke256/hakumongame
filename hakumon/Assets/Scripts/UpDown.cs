using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class UpDownPlatform : MonoBehaviour
{
    [Header("上に移動する距離")]
    [Min(0f)]
    public float moveDistance = 4f;
    [Header("移動速度")]
    [Min(0f)]
    public float moveSpeed = 2f;
    private Rigidbody2D rb;
    private Vector2 bottomPosition;
    private bool movingUp = true;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // 重力の影響を受けず、スクリプトで動かす
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        // 配置した場所を下端にする
        bottomPosition = rb.position;
    }
    private void FixedUpdate()
    {
        if (moveDistance <= 0f || moveSpeed <= 0f) return;
        Vector2 topPosition =
            bottomPosition + Vector2.up * moveDistance;
        Vector2 target =
            movingUp ? topPosition : bottomPosition;
        Vector2 nextPosition = Vector2.MoveTowards(
            rb.position,
            target,
            moveSpeed * Time.fixedDeltaTime
        );
        rb.MovePosition(nextPosition);
        // 端に到着したら方向を反転
        if ((nextPosition - target).sqrMagnitude < 0.000001f)
        {
            movingUp = !movingUp;
        }
    }
}