
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("追従するプレイヤー")]
    [SerializeField] private Transform player;

    [Header("カメラ追従の滑らかさ")]
    [SerializeField] private float smoothTime = 0.3f;

    [Header("プレイヤーより右を映す距離")]
    [SerializeField] private float offsetX = 4f;

    [Header("ステージ左端のX座標")]
    [SerializeField] private float stageLeftX = -15f;

    private Camera cam;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        cam = GetComponent<Camera>();

        if (player == null)
        {
            Debug.LogError("Playerが設定されていません！");
            return;
        }

        // ゲーム開始時はプレイヤーを画面中央に映す
        Vector3 pos = transform.position;
        pos.x = player.position.x;
        transform.position = pos;
    }

    void LateUpdate()
    {
        if (player == null || cam == null)
        {
            return;
        }

        Vector3 targetPos = new Vector3(
            CalculateCameraX(),
            transform.position.y,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            Mathf.Max(0.001f, smoothTime)
        );
    }

    float CalculateCameraX()
    {
        // カメラの画面幅の半分
        float halfWidth =
            cam.orthographicSize * cam.aspect;

        // 画面左端がステージ左端より
        // 左に出ないようにする
        float minimumCameraX = stageLeftX + halfWidth;

        // プレイヤーを追従
        float targetX = player.position.x + offsetX;

        return Mathf.Max(targetX, minimumCameraX);
    }
}
