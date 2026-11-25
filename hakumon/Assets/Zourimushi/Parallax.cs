using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Transform targetCamera;

    // カメラの移動に対する背景の移動倍率
    [SerializeField] private float parallaxFactor = 0.5f;

    private Vector3 previousCameraPosition;

    private void Start()
    {
        previousCameraPosition = targetCamera.position;
    }

    private void LateUpdate()
    {
        // カメラが実際に移動した距離を取得
        Vector3 cameraDelta = targetCamera.position - previousCameraPosition;

        // カメラの移動量に倍率を掛けて背景を動かす
        transform.position += cameraDelta * parallaxFactor;

        previousCameraPosition = targetCamera.position;
    }
}