using UnityEngine;

public class WallChecker : MonoBehaviour
{
    // EnemyのSpriteRenderer
    [SerializeField] private SpriteRenderer enemySr;
    // 判定対象のレイヤー（Inspectorで指定）
    [SerializeField] private LayerMask groundLayer;

    // オブジェクトに触れた瞬間を検知
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 接触したオブジェクトのレイヤーが groundLayer に含まれているか判定
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            enemySr.flipX = !enemySr.flipX;
        }
    }
}