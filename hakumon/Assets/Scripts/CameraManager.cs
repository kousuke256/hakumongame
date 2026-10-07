using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float limitX = 100f;

    // 遅れの強さ（大きいほど遅れる）
    [SerializeField] private float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (player == null) return;

        float targetX = Mathf.Clamp(player.position.x+2f, 0f, limitX);
        Vector3 targetPos = new Vector3(targetX, transform.position.y, transform.position.z);

        // 遅れて追従する
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );
    }
}