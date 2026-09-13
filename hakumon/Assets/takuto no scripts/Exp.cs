using UnityEngine;
using Unity.Mathematics;
public class Exp : MonoBehaviour
{
    public float maxSpeed = 20f;
    public Transform player;
    public Player playerScript;
    public GameObject bullet2Prefab;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(ShootBullet2), 1.2f, 0.3f);
    }

    void ShootBullet2()
    {
        float speed;
        int bulletDirectionX = 1;
        float playerGravityDirection = -playerScript.gravityDirection.y;
        float differenceX = player.position.x - transform.position.x;
        float differenceY = playerGravityDirection * (player.position.y - transform.position.y);
        if(differenceX < 0)
        {
            differenceX *= -1f;
            bulletDirectionX = -1;
        }
        int i = 0;
        float theta = Mathf.Atan((differenceY + Mathf.Sqrt(differenceX * differenceX + differenceY * differenceY)) / differenceX);
        float tan = math.tan(theta);
        float maxHeight = differenceX * differenceX * tan * tan / (4 * (differenceX * tan - differenceY));
        while(transform.position.y * playerGravityDirection + maxHeight > 5f)
        {
            i++;
            theta -= Mathf.Deg2Rad * 10f;
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
            speed = maxSpeed < speed ? maxSpeed : speed;
            Debug.Log(i);
        }
        

        GameObject bullet = Instantiate(bullet2Prefab, transform.position, quaternion.identity);
        bullet.GetComponent<Bullet2>().Shoot(bulletDirectionX, playerGravityDirection, speed, theta);
    }
}
