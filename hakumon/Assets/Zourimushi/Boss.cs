
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;

public class Boss : MonoBehaviour
{
    public enemystate state;
    public Transform player;
    public Rigidbody2D playerRb;
    public Player playerScript;
    public GameObject bullet1Prefab;
    public GameObject bullet2Prefab;
    public Transform firePoint;
    public float bullet1Speed = 8f; //　playerの速度より弾速を速くしないとだめ
    public float bulletMaxSpeed = 20f;
    private int creatBulletCounter = 0;
    private int kirikae = 1;
    public int kirikaeCounter = 5;
    public float ceiling = 5.1f;
    private float moveDirection;
    public float speed = 2.3f;
    private bool canWalk = true;
    public float jumpPower = 5.4f;
    public int maxTrun = 3;
    private int turnCount = 0;
    public float gravityPower = 9.8f;
    public  float shootCoolTime1 = 0.6f;
    public  float shootCoolTime2 = 0.6f;
    private Vector2 gravityDirection = Vector2.right;

    private Rigidbody2D rb;

    int[] rand = {0, 5, -5};

    int[] directions = {1, -1};

    public enum enemystate
    {
        walk,
        JumpAtack,
        ShootAtack1,
        ShootAtack2,
        GravityAtack,
        Break,
        Freeze,

    }

    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        UpdateRotation();
        moveDirection = directions[UnityEngine.Random.Range(0, directions.Length)];
        Debug.Log(gravityDirection);
        InvokeRepeating(nameof(ShootBullet1), 0.5f, shootCoolTime1);
        //InvokeRepeating(nameof(ShootBullet2), 0.5f, shootCoolTime2);
        
    }

    void FixedUpdate()
    {
        Debug.Log(math.dot(playerRb.linearVelocity, playerScript.gravityDirection));
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

    /*private void ShootBullet1()
    {
        int bulletDirectionX = 1;
        float distanceX = player.position.x - firePoint.position.x;
        float distanceY = player.position.y - firePoint.position.y;
        if(distanceX < 0)
        {
            distanceX *= -1f;
            bulletDirectionX = -1;
        }
        float theta = Mathf.Atan(distanceY / distanceX) + Mathf.Asin((distanceX * playerRb.linearVelocity.y - distanceY * playerRb.linearVelocity.x) / (bullet1Speed * Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY)));

        //new Vector2(player.position.x - firePoint.position.x , player.position.y - firePoint.position.y + UnityEngine.Random.Range(-1.5f,1.5f));
        GameObject bullet = Instantiate(bullet1Prefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet1>().ShootBullet(bullet1Speed, theta, bulletDirectionX);
    }*/
    private void ShootBullet1()
    {
        //　playerの速度より弾速を速くしないとだめ
        Vector2 distance = player.position - firePoint.position;
        Vector2 playerVelocity = playerRb.linearVelocity;

        // 二次方程式
        float A = playerVelocity.sqrMagnitude - bullet1Speed * bullet1Speed;
        float B = Vector2.Dot(distance, playerVelocity);
        float C = distance.sqrMagnitude;
        float sqrtDiscriminant = Mathf.Sqrt(B * B - A * C);

        float time = (-B + sqrtDiscriminant) / A;
        if (time < 0f)
            time = (-B - sqrtDiscriminant) / A;
            
        Vector2 targetPosition = (Vector2)player.position + playerVelocity * time;
        if(targetPosition.y > 4.6f)
        {
            float y = targetPosition.y - 4.6f;

        }
        else if(targetPosition.y < -4.6f)
        {
            targetPosition.y = -4.6f;
        }
        Vector2 direction = targetPosition - (Vector2)firePoint.position;
        GameObject bullet = Instantiate(bullet1Prefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet1>().ShootBullet(bullet1Speed, direction);
    }


    void ShootBullet2()
    {
        creatBulletCounter++;
        float speed;
        int bulletDirectionX = 1;
        float playerGravityDirection = 1; //= -playerScript.gravityDirection.y;

        //銃弾の重力が順番に切り替わるようにするために後から追加してみたやつ
        
        if(creatBulletCounter % (kirikaeCounter + 1) == 0)
        {
            kirikae *= -1;
            return;
        }
        playerGravityDirection *= kirikae;
        

        float differenceX = player.position.x - firePoint.position.x;
        float differenceY = playerGravityDirection * (player.position.y - firePoint.position.y);

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
        while(firePoint.position.y * playerGravityDirection + maxHeight > ceiling)
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