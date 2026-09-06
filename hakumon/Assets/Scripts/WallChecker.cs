using UnityEngine;

public class WallChecker : MonoBehaviour
{
    //EnemyのSpriteRenderer
    [SerializeField] private SpriteRenderer enemySr;
    //オブジェクトに触れた瞬間を検知
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //もし壁に当たると
        if(collision.gameObject.CompareTag("Ground"))
        {
            enemySr.flipX = !enemySr.flipX ;
        }
    }


}