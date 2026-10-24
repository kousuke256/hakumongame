using UnityEngine;
public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    // カメラが追従する際の遅れ
    [SerializeField] private float smoothTime = 0.3f;
    // プレイヤーより右側を映す距離
    [SerializeField] private float offsetX = 2f;
    // カメラの移動範囲
    [SerializeField] private float FirstX = 0f;
    [SerializeField] private float MaxX = 500f;
    private Vector3 velocity = Vector3.zero;
    private bool initialized = false;
    private void Start()
    {
        // 未設定ならPlayerタグで探す
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }
    private void LateUpdate()
    {
        if (player == null) return;
        float targetX = Mathf.Clamp(
            player.position.x + offsetX,
            FirstX,
            MaxX
        );
        Vector3 targetPos = new Vector3(
            targetX,
            transform.position.y,
            transform.position.z
        );
        // PlayerのStartで復活位置へ移動した後に合わせる
        if (!initialized)
        {
            transform.position = targetPos;
            velocity = Vector3.zero;
            initialized = true;
            return;
        }
        // その後は滑らかに追従
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );
    }
}