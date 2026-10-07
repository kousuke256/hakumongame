using UnityEngine;
using Unity.Mathematics;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public Transform player;
    public Transform firePoint;
    public GameObject bullet2Prefab;
    public int bulletGravityDirection;
    public float ceiling;

    public float bulletMaxSpeed = 20f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Throw()
    {
        float speed;
        int bulletDirectionX = 1;

        float differenceX = player.position.x - firePoint.position.x;
        float differenceY = bulletGravityDirection * (player.position.y - firePoint.position.y);

        if(differenceX < 0)
        {
            differenceX *= -1f;
            bulletDirectionX = -1;
        }
        //変数名考えるのめんどくさかったから許して、そもそもintじゃなくてboolにしとけばよかったと後悔してる
        int i = 0;
        float theta = Mathf.Atan((differenceY + Mathf.Sqrt(differenceX * differenceX + differenceY * differenceY)) / differenceX);
        float tan = math.tan(theta);
        float maxHeight = differenceX * differenceX * tan * tan / (4 * (differenceX * tan - differenceY));
        while(firePoint.position.y * bulletGravityDirection + maxHeight > ceiling)
        {
            i++;
            theta -= Mathf.Deg2Rad * 3f;
            tan = math.tan(theta);
            maxHeight = differenceX * differenceX * tan * tan / (4 * (differenceX * tan - differenceY));
        }
        if(i == 0)
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
