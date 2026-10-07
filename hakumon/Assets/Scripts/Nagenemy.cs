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
        InvokeRepeating(nameof(Throw), shootStartDelay, shootCoolTime);
    }

    void Update()
    {
        if(!canMove)
        return;

        if (player.position.x > transform.position.x)
        {
            // Playerが右にいる
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            // Playerが左にいる
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

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

        float speed;
        int bulletDirectionX = 1;

        float differenceX = player.position.x - firePoint.position.x;
        float differenceY = bulletGravityDirection * (player.position.y - firePoint.position.y);

        if(differenceX < 0)
        {
            differenceX *= -1f;
            bulletDirectionX = -1;
        }
        bool a = true;
        float theta = Mathf.Atan((differenceY + Mathf.Sqrt(differenceX * differenceX + differenceY * differenceY)) / differenceX);
        float tan = math.tan(theta);
        float maxHeight = differenceX * differenceX * tan * tan / (4 * (differenceX * tan - differenceY));
        while(firePoint.position.y * bulletGravityDirection + maxHeight > ceiling)
        {
            a = false;
            theta -= Mathf.Deg2Rad * 3f;
            tan = math.tan(theta);
            maxHeight = differenceX * differenceX * tan * tan / (4 * (differenceX * tan - differenceY));
        }
        if(a)
        {
            speed = math.sqrt(10 * (differenceY + math.sqrt(differenceX * differenceX + differenceY * differenceY)));
        }
        else
        {
            speed = (differenceX / math.cos(theta) * math.sqrt(5 / math.abs(differenceX * tan - differenceY)));
            speed = bulletMaxSpeed < speed ? bulletMaxSpeed : speed;
        }

        GameObject bullet = Instantiate(bullet2Prefab, firePoint.position, quaternion.identity);
        bullet.GetComponent<Bullet2>().Shoot(bulletDirectionX, bulletGravityDirection, speed, theta);
    }
}
