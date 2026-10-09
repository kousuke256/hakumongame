using UnityEngine;
using Unity.Mathematics;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("弾の情報")]
    public float shootStartDelay;
    public float shootCoolTime;
    public float bulletMaxSpeed = 20f;
    public int bulletGravityDirection;
    public float ceiling;
    public Transform player;
    public Transform firePoint;
    public GameObject bullet2Prefab;
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
        if(!canMove)
        return;

        // これ以降のプログラムはほぼ弾道の計算がメイン
        /* おおまかな仕組みを説明すると、playerの位置に弾が届く最小速度にの発射角度を求めた後、
        その弾道の最高到達点が天井にぶつからないように発射角度を低くしていく。
        そして、その発射角度の場合どのくらいの初速度が必要かを計算してbullet2のshootメゾットに渡している */
        float speed;
        int bulletDirectionX = 1;

        // playerと球の発射位置の距離を求める
        float differenceX = player.position.x - firePoint.position.x;
        float differenceY = bulletGravityDirection * (player.position.y - firePoint.position.y);
        if(differenceX < 0)
        {
            differenceX *= -1f;
            bulletDirectionX = -1;
        }

        // playerの位置に弾が届く最小速度にの発射角度を求める
        float theta = Mathf.Atan((differenceY + Mathf.Sqrt(differenceX * differenceX + differenceY * differenceY)) / differenceX);
        float tan = math.tan(theta);
        float maxHeight = differenceX * differenceX * tan * tan / (4 * (differenceX * tan - differenceY));

        //天井に当たらないように発射角度を3度ずつ下げてく
        while(firePoint.position.y * bulletGravityDirection + maxHeight > ceiling)
        {
            theta -= Mathf.Deg2Rad * 3f;
            tan = math.tan(theta);
            maxHeight = differenceX * differenceX * tan * tan / (4 * (differenceX * tan - differenceY));
        }

        // 速度を求める
        speed = (differenceX / math.cos(theta) * math.sqrt(5 / math.abs(differenceX * tan - differenceY)));
        speed = bulletMaxSpeed < speed ? bulletMaxSpeed : speed;

        GameObject bullet = Instantiate(bullet2Prefab, firePoint.position, quaternion.identity);
        bullet.GetComponent<Bullet2>().Shoot(bulletDirectionX, bulletGravityDirection, speed, theta);
    }
}
