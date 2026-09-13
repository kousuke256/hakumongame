using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;

public class Boss : MonoBehaviour
{
    public enemystate state;
    public Transform player;
    public Player playerScript;
    public GameObject bullet1Prefab;
    public GameObject bullet2Prefab;
    public Transform firePoint;
    public float bulletMaxSpeed = 20f;
    private float moveDirection;
    public float speed = 2.3f;
    private bool canWalk = true;
    public float jumpPower = 5.4f;
    public int maxTrun = 3;
    private int turnCount = 0;
    public float gravityPower = 9.8f;
    public  float shootCoolTime = 0.6f;
    private Vector2 gravityDirection = Vector2.right;

    private Rigidbody2D rb;

    int[] directions =
    {
        1,
        -1,
    };

    public enum enemystate
    {
        walk,
        JumpAtack,
        ShootAtack1,
        ShootAtack2,
        GravityAtack,
        Freeze,

    }

    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        UpdateRotation();
        moveDirection = directions[UnityEngine.Random.Range(0, directions.Length)];
        Debug.Log(gravityDirection);
       // InvokeRepeating(nameof(ShootBullet1), 1f, shootCoolTime);
        InvokeRepeating(nameof(ShootBullet2), 1.2f, shootCoolTime);
        
    }

    void FixedUpdate()
    {
        rb.AddForce(gravityDirection * gravityPower);
        if (canWalk)
        {
            Walk();
        }
    }

    void SetState()
    {
        if(state == enemystate.walk)
        {
            
        }
    }

    private void ShootBullet1()
    {
        GameObject bullet = Instantiate(bullet1Prefab, firePoint.position, Quaternion.identity);
        Vector2 direction = player.position - firePoint.position;
        bullet.GetComponent<Bullet1>().ShootBullet(direction);
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
            speed = bulletMaxSpeed < speed ? bulletMaxSpeed : speed;
        }

        GameObject bullet = Instantiate(bullet2Prefab, transform.position, quaternion.identity);
        bullet.GetComponent<Bullet2>().Shoot(bulletDirectionX, playerGravityDirection, speed, theta);
    }

    void GravityChenge()
    {
        gravityDirection = new Vector2(-moveDirection * gravityDirection.y, moveDirection * gravityDirection.x);
        UpdateRotation();
    }
     void UpdateRotation()
    {
        Vector2 upDirection = -gravityDirection;
        transform.up = upDirection;
    }

    void Walk()
    {
        Vector2 velocity = rb.linearVelocity;
        Vector2 moveVelocity = transform.right * moveDirection * speed;
        float gravitySpeed = Vector2.Dot(velocity, gravityDirection);
        Vector2 gravityVelosity = gravitySpeed * gravityDirection;
        rb.linearVelocity = gravityVelosity + moveVelocity;
        
    }

    public void JumpOrTurn()
    {
        if(UnityEngine.Random.Range(0, 2) == 0 || turnCount == maxTrun)
        {
            turnCount = 0;
            rb.linearVelocity = transform.up * jumpPower;
            Invoke(nameof(GravityChenge), 0.4f);
        }
        else
        {
            moveDirection *= -1;
            turnCount++;
        }
    }

    void CantWalk()
    {
        canWalk = false;
    }
}