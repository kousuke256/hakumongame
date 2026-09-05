using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //プレイヤーのTransform
    [SerializeField] private Transform player;

    //カメラが移動できる最大値
    [SerializeField] private float limitX = 100.0f;

    private void LateUpdate()
    {
        FollowPlayer();
    }
    
    //プレイヤーを追いかけるメソッド
    private void FollowPlayer()
    {
        //プレイヤーのTransformのデータがなければ
        if (player == null)
        {
            Debug.Log("PlayerのTransformのデータがありません");
            return;
        }
        //Mathf.Clampを使って、プレイヤーのX座標を0からlimitXの間に収めた値をclampedXに代入する
        //Mathf.Clamp(制限する対象,最小値,最大値)
        float clampedX = Mathf.Clamp(player.position.x,0f,limitX);
        //カメラ位置を変更,y,z,は現状維持
        transform.position = new Vector3(clampedX,transform.position.y,transform.position.z);
    }
}
