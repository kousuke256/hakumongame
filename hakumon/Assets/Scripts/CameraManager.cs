using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform player;

    // カメラが追従する際の遅れ
    [SerializeField] private float smoothTime = 0.3f;

    // プレイヤーより右側を映す距離
    [SerializeField] private float offsetX = 4f;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (player == null) return;

        // X方向の追従（上限なし）
        float targetX = Mathf.Max(
            player.position.x + offsetX,
            0f
        );

        Vector3 targetPos = new Vector3(
            targetX,
            transform.position.y,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );
    }
}