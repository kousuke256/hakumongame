
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;

public class Boss : MonoBehaviour
{
    // canWalkをtrueにFixedUpdateのコメント消す
    public bossState state = bossState.walk;
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
    private int moveDirection = 1;
    public float speed = 3.5f;
    public float chaseSpeed = 6f;
    public float chaseAcceleration = 1f;
    public float jumpPower = 5.4f;
    public int maxTrun = 3;// walk状態は3, chase状態は0に
    private int turnCount = 0;
    public float gravityPower = 9.8f;
    public  float shootCoolTime1 = 0.6f;
    public  float shootCoolTime2 = 0.6f;
    private Vector2 gravityDirection = Vector2.right;
    private float patternTimer = 100f;
    private float shootTimer = 0f;
    private float patternChangeTime;
    private bossState previousState = bossState.Freeze;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    public enum bossState
    {
        walk,
        chase,
        ShootAtack1,
        ShootAtack2,
        GravityAtack,
        Break,
        Freeze,

    }

    void SetState()// 変数名考えるのめんどい
    {
        switch (state)
        {
            case bossState.walk :
                shootTimer = 0f;
                patternChangeTime = 12.5f;
                do
                {
                    state = (bossState)UnityEngine.Random.Range(2, 4);
                }
                while(state == previousState);
                previousState = state;
                break;


            case bossState.ShootAtack1 : 
            case bossState.ShootAtack2 :
                goto case bossState.chase;

            case bossState.chase : 
                patternChangeTime = 1.5f;
                state = bossState.walk;////////////////////////////////////////////////////
                break;

                case bossState.Freeze : 
                break;

        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        UpdateRotation();
        SetState();
        //jumpPower = 10f;
    }

    void Update()
    {
        patternTimer += Time.deltaTime;
        
        if(patternTimer > patternChangeTime)
        {
            patternTimer = 0f;
            SetState();
        }

        if(state == bossState.ShootAtack1)
        {
            shootTimer += Time.deltaTime;
            if(shootTimer > shootCoolTime1)
            {
                shootTimer = 0f;
                ShootBullet1();
            }
        }
        else if(state == bossState.ShootAtack2)
        {
            shootTimer += Time.deltaTime;
            if(shootTimer > shootCoolTime2)
            {
                shootTimer = 0f;
                ShootBullet2();
            }
        }


    }

    void FixedUpdate()
    {
        rb.AddForce(gravityDirection * gravityPower);
        
        if (state == bossState.chase)
        {
            chase();
        }
        else
        {
            Walk();
        }
    }

    void chase()
    {
        
        if (gravityDirection.y == 0)
        {
            // 重力が左右

            float targetSpeed = playerScript.gravityDirection.y > 0 ? chaseSpeed : -chaseSpeed;
            float newY = Mathf.MoveTowards(rb.linearVelocity.y, targetSpeed, chaseAcceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, newY);
        }
        else
        {
            // 重力が上下
            float targetSpeed = player.position.x > transform.position.x ? chaseSpeed : -chaseSpeed;
            float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, chaseAcceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
        }
    }

    private void ShootBullet1()// -4.58カラ-0.26までジャンプした
    {
        int a = 0;// aで色変えれる.bullet1の
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
        float playerGravityDirection = playerScript.gravityDirection.y;
        if(targetPosition.y * playerGravityDirection > ceiling)// 0.3fはplayerの大きさを何となく加味した数字（本当は0.4fにすべきなんだろうが）
        {
            time = Mathf.Abs((player.position.y - (ceiling - 0.3f) * playerGravityDirection) / playerRb.linearVelocity.y);
            targetPosition = new Vector2(player.position.x + playerRb.linearVelocity.x * time, 4.7f * playerGravityDirection);
        }
        else if(math.dot(playerScript.gravityDirection.y, playerRb.linearVelocity) < -2f && targetPosition.y > -0.6f && playerGravityDirection == -1)
        {
            a= 1;
            targetPosition.y = -0.7f;
        }
        else if(math.dot(playerScript.gravityDirection.y, playerRb.linearVelocity) < -2f && targetPosition.y < 0.6f && playerGravityDirection == 1)
        {
            a=1;
            targetPosition.y = 0.7f;
        }
        Vector2 direction = targetPosition - (Vector2)firePoint.position;
        GameObject bullet = Instantiate(bullet1Prefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet1>().ShootBullet(bullet1Speed, direction, a);
    }


    void ShootBullet2()
    {
        creatBulletCounter++;
        float speed;
        int bulletDirectionX = 1;
        float playerGravityDirection = 1; //= -playerScript.gravityDirection.y;

        //銃弾の重力が順番に切り替わるようにするために後から追加してみたやつ
        
        if(creatBulletCounter > kirikaeCounter)
        {
            creatBulletCounter = 0;
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
        Vector2 moveVelocity = transform.right * moveDirection * speed;
        float gravitySpeed = Vector2.Dot(rb.linearVelocity, gravityDirection);
        Vector2 gravityVelosity = gravitySpeed * gravityDirection;
        rb.linearVelocity = gravityVelosity + moveVelocity;
        
    }

    public void JumpOrTurn()
    {
        if(state == bossState.chase)
        return;

        if(UnityEngine.Random.Range(0, 2) == 0 || turnCount == maxTrun)
        {
            turnCount = 0;
            rb.linearVelocity = transform.up * jumpPower;
            GravityChenge();
        }
        else
        {
            moveDirection *= -1;
            turnCount++;
        }
    }
}