using UnityEngine;

public class GravityBlock : MonoBehaviour
{
    public Player playerScript;
    public Transform rail;
    public float moveSpeed = 3f;
    private float railHeight;
    private float minY = -5f;
    private float maxY = 5f;
    public bool hirameki  = true;
    private Rigidbody2D rb;
    private bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // 重力による自然落下を無効にする
        rb.gravityScale = 0f;

        // レールのX座標に固定する
        rb.position = new Vector2(rail.position.x, rb.position.y);

        railHeight = rail.localScale.y/2;
        maxY = rail.position.y + railHeight;
        minY = rail.position.y - railHeight;
    }

    void FixedUpdate()
    {
        if(!canMove && !hirameki)
        return;

        // Playerの重力方向を取得する
        Vector2 gravityDirection = playerScript.gravityDirection;

        // 上下方向の重力にだけ反応する
        if (gravityDirection.y != 0)
        {
            // 重力方向に応じてY座標を動かす
            float newY = rb.position.y + gravityDirection.y * moveSpeed * Time.fixedDeltaTime;

            // 移動範囲を制限する
            newY = Mathf.Clamp(newY, minY, maxY);

            // X座標を固定し、Y座標だけ変更する
            rb.MovePosition(new Vector2(rail.position.x, newY));
        }
    }

    void OnBecameVisible()
    {
        canMove = true;
    }
    void OnBecameInvisible()
    {
        canMove = false;
    }
}