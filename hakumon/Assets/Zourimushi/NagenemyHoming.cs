using UnityEngine;
using Unity.Mathematics;

public class NagenemyHoming : MonoBehaviour
{
    [Header("弾の情報")]
    public float shootStartDelay;
    public float shootCoolTime;

    public Transform player;
    public Transform firePoint;
    public GameObject homingBullet;
    private bool canMove;


    void Start()
    {
        // shootStartDelay秒後に弾を投げ始めて、shootCoolTime秒間隔で投げる
        InvokeRepeating(nameof(Throw), shootStartDelay, shootCoolTime);
    }

    void Update()
    {
        if(!canMove)
        return;

        if (player.position.x > transform.position.x)
        {
            // Playerが右にいる場合は右を向く
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            // Playerが左にいる場合は左を向く
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // 画面内にこいつがいる場合は動作を可能にして、画面外にいる場合は動作を不能にする
    void OnBecameVisible()
    {
        canMove = true;
    }
    void OnBecameInvisible()
    {
        canMove = false;
    }

    void Throw()
    {
        // 弾を生成する
        GameObject bullet = Instantiate(homingBullet, firePoint.position, Quaternion.identity);
    }
}
