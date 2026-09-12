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
        float maxBulletSpeed = 17f;
        bool canNanameShoot = true;
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
        if(differenceX < 8f)
        {
            maxBulletSpeed = 12f;
        }
        else if (differenceX < 12f)
        {
            maxBulletSpeed = 15f;
        }
        //めんどくさくて数字使っちゃったけど許して♡、要するに天井まで距離あったらっていうif文
        if(playerGravityDirection * transform.position.y + 1.8f < 4.5f)
        {
            speed = differenceX * math.sqrt(10f /math.abs(differenceX - differenceY));
        }
        else
        {
            canNanameShoot = false;
            speed = differenceX * math.sqrt(10f / math.abs(2f * differenceY) );
        }
        if(speed > maxBulletSpeed)
        {
        speed = maxBulletSpeed;
        }
        GameObject bullet = Instantiate(bullet2Prefab, transform.position, quaternion.identity);
        bullet.GetComponent<Bullet2>().Shoot(bulletDirectionX, playerGravityDirection, speed, canNanameShoot);
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