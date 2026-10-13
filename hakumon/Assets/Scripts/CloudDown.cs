using UnityEngine;

public class CloudDown : MonoBehaviour
{
    PlatformEffector2D effector;
    Player player;  // ← Player を参照する
    Collider2D col;  
    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();
        player = FindAnyObjectByType<Player>();
        col = GetComponent<Collider2D>();
    }
    void Update()
    {
        // Player の gravityDirection を使って Rotational Offset を変更
        // 方向ベクトルを角度に変換する
        float angle = Vector2.SignedAngle(Vector2.down, player.gravityDirection);
        if (player.gravityDirection == Vector2.down)
        {
             col.enabled = false;
        }
        else
        {
             col.enabled = true;
            effector.rotationalOffset = angle;
        }
       
    }
}
