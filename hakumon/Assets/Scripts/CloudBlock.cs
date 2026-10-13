using UnityEngine;

public class CloudBlock : MonoBehaviour
{
    PlatformEffector2D effector;
    Player player;  // ← Player を参照する

    void Start()
    {
        effector = GetComponent<PlatformEffector2D>();

        // シーン内の Player を探す（1人だけならこれでOK）
        player = FindAnyObjectByType<Player>();
    }

    void Update()
    {
        // Player の gravityDirection を使って Rotational Offset を変更
        // 方向ベクトルを角度に変換する
        float angle = Vector2.SignedAngle(Vector2.down, player.gravityDirection);

        effector.rotationalOffset = angle;
    }
}
