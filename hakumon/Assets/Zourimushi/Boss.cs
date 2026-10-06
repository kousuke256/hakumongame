

using System;
using System.Runtime.InteropServices;
using NUnit.Framework.Internal.Commands;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;

public class Boss : MonoBehaviour
{
    public bossState state = bossState.Freeze;
    public Transform player;
    public Rigidbody2D playerRb;
    public Player playerScript;
    public GameObject bullet1Prefab;
    public GameObject bullet2Prefab;
    public Transform firePoint;
    public float bullet1Speed = 8f; //　playerの速度より弾速を速くしないとだめ
    public float bulletMaxSpeed = 20f;
    private int creatBulletCounter = 0;
    private float bulletGravityDirection = 1;
    public int kirikaeCounter = 5;
    private int skipCounter;
    public float ceiling = 5.1f;
    private int moveDirection = 1;
    public float walkSpeed = 3.5f;
    public float chaseSpeed = 6f;
    public float chaseAcceleration = 1f;
    public float chaseJumpPower = 8f;
    private bool canChaseJump = true;
    public float walkJumpPower = 5.4f;
    public int maxTurn = 3;
    private int turnCount = 0;
    public float gravityPower = 9.8f;
    public  float shootCoolTime1 = 0.6f;
    public  float shootCoolTime2 = 0.6f;
    public Vector2 gravityDirection;
    private float patternTimer = 0f;
    private float shootTimer = 0f;
    private float patternChangeTime = 1f;

    [Header("設置判定")]

    public bool isGrounded;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bossState previousState = bossState.Freeze;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    
float wallPlayerDistance;
    float groundBossDistance;

    public enum bossState
    {
        walk,
        chase,
        chaseStun,
        ShootAtack1,
        ShootAtack2,
        GravityAtack,
        Freeze,

    }

    // paternChangeTimeで次の行動が何秒続くかを決め、stateを変えている
    void SetState()
    {
        switch (state)
        {
            case bossState.walk :
                shootTimer = 0f;
                patternChangeTime = 12.5f;
                // bossの攻撃をShootAtack1かShootAtack2にする。同じ行動が連続で出ないようにしてある
                do{
                    state = (bossState)UnityEngine.Random.Range(3,5);
                }while(state == previousState);
                previousState = state;
                break;

            case bossState.ShootAtack1 : 
            case bossState.ShootAtack2 :
                goto case bossState.chase;

            case bossState.chase : 
                patternChangeTime = 1f;
                state = bossState.walk;
                break;

            case bossState.chaseStun :
                patternChangeTime = 100f;
                state = bossState.chase;
                break;

            case bossState.Freeze : 
                patternChangeTime = 0.5f;
                state = bossState.walk;
                break;

        }
    }

    void Start()
    {
        gravityDirection = Vector2.right;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Debug.Log( "move:" + moveDirection);
        patternTimer += Time.deltaTime;
        
        if(patternTimer > patternChangeTime)
        {
            // patternTimerがpatternChangetimeになったらbossStateをかえる
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
        // 接地判定
        isGrounded = Physics2D.OverlapCircle(groundCheck.position,groundCheckRadius,groundLayer);

        // 重力, rigidbodyのgravityScale=0だから
        rb.AddForce(gravityDirection * gravityPower);

        if(state == bossState.Freeze)
        return;
        
        if (state == bossState.chase)
        {
            chase();
        }
        else
        {
            Walk(walkSpeed);
        }
    }

    void Walk(float speed)
    {
        // moveDirectioが1なら重力方向の右に、-1なら左に進む

        Vector2 moveVelocity = transform.right * moveDirection * speed;

        // 重力方向のbossのずぴーどを取得してmoveVelocityと合成
        float gravitySpeed = Vector2.Dot(rb.linearVelocity, gravityDirection);
        Vector2 gravityVelosity = gravitySpeed * gravityDirection;
        rb.linearVelocity = gravityVelosity + moveVelocity;
        
    }

    void Jump(float jumpPower)
    {
        Vector2 velocity = rb.linearVelocity;

        // 重力方向の速度を取得
        float gravityVelocity = Vector2.Dot(velocity, gravityDirection);
    
        // 重力方向の速度を取り除き、ジャンプの力を加える
        velocity -= gravityDirection * gravityVelocity;
        velocity += -gravityDirection * jumpPower;

        rb.linearVelocity = velocity;
    }

    // wallCheckerが壁にぶつかると実行される
    public void JumpOrTurn()
    {
        if(state == bossState.chase)
        return;
        
        // 1/2の確率でジャンプする
        if(UnityEngine.Random.Range(0, 2) == 0 || turnCount == maxTurn)
        {
            turnCount = 0;
            Jump(walkJumpPower);
            GravityChengeLeftOrRight();
        }
        else
        {
            // 移動方向を反転させる。　連続のturn回数がmaxTurnになると行動パターンが絶対jumpになる
            moveDirection *= -1;
            turnCount++;
        }
    }

    void GravityChengeLeftOrRight()
    {
        // 現在進んでいる方向が重力の方向になり、オブジェクトの上を重力と逆向きにする
        gravityDirection = new Vector2(-moveDirection * gravityDirection.y, moveDirection * gravityDirection.x);
        transform.up = -gravityDirection;
    }

    void chase()
    {
        // 重力が左右だったら
        if (gravityDirection.y == 0)
        {
            // moveDirectionの方向に慣性のある移動をする
            float newY = Mathf.MoveTowards(rb.linearVelocity.y, moveDirection * chaseSpeed, chaseAcceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, newY);

            // 落下攻撃を可能にする
            canChaseJump = true;

            if(!isGrounded)
            return;

            // bossが張り付いている壁とplayerの距離が近いのにもかかわらずbossが高いところにいたらジャンプできないようにするif文
            wallPlayerDistance = 10 - gravityDirection.x * player.position.x;
            groundBossDistance = 5 - gravityDirection.x * moveDirection * transform.position.y;
            if(wallPlayerDistance - groundBossDistance < 1f)
            return;

            // bossが高いところからジャンプするほどジャンプ力が高くなる
            // groundBossDistanceに5を足しているのはどれだけ地面からの距離が近くてもある程度ジャンプさせるため
            Jump(chaseJumpPower * (groundBossDistance + 5f) / 8f);
            GravityChengeLeftOrRight();
        }
        else
        {
            // 重力が上下
            if (isGrounded)
            {
                if(gravityDirection != playerScript.gravityDirection && !canChaseJump)
                {
                    Walk(chaseSpeed);

                    // 進んでいる方向にある壁からの距離が6より大きかった場合return;
                    float wallBossDistance = transform.position.x + 10 * moveDirection * gravityDirection.y;
                    if(wallBossDistance > 6f)
                    return;

                    // playerと地面との距離が5超過の場合returnを実行してジャンプできないようにしている
                    if(5 - playerScript.gravityDirection.y * player.position.y > 5f)
                    return;

                    // 進んでいる方向にある壁からの距離に比例してジャンプ力が高くなる
                    // wallBossDistanceに1を足しているのはどれだけ壁からの距離が近くてもある程度ジャンプさせるため
                    Jump(chaseJumpPower * (wallBossDistance + 1f) / 4f);
                    GravityChengeLeftOrRight();
                    return;
                }

                // 慣性のある移動をしながらプレイヤーを追う
                int wowowoMoveDirection = player.position.x > transform.position.x ? 1 : -1;
                float newX = Mathf.MoveTowards(rb.linearVelocity.x, wowowoMoveDirection * chaseSpeed, chaseAcceleration * Time.fixedDeltaTime);
                rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
                moveDirection = gravityDirection.y == -1 ? wowowoMoveDirection : -wowowoMoveDirection;

                if(!canChaseJump)
                return;

                if(gravityDirection == playerScript.gravityDirection)
                return;

                

                // 0.7fはBossの落下時間, playerの位置を予測してbossが天井から落ちてくるプログラム
                float playerFuturePosiition = player.position.x + playerRb.linearVelocity.x * 0.7f;
                float bossFuturePosiition = transform.position.x + rb.linearVelocity.x * 0.7f;
                if(Mathf.Abs(playerFuturePosiition - bossFuturePosiition) < 0.3f)
                {
                    // 進んでいる方向にある壁からの距離に比例してジャンプ力が高くなる
                    // wallBossDistanceに1を足しているのはどれだけ壁からの距離が近くてもある程度ジャンプさせるため
                    Jump(chaseJumpPower);
                    gravityDirection *= -1;
                    transform.up = -gravityDirection;
                    moveDirection *= -1;
                    canChaseJump = false;
                }
            }
        }
    }

    private void ShootBullet1()// -4.58カラ-0.26までジャンプした
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
        float playerGravityDirection = playerScript.gravityDirection.y;
        if(targetPosition.y * playerGravityDirection > ceiling)// 0.3fはplayerの大きさを何となく加味した数字（本当は0.4fにすべきなんだろうが）
        {
            time = Mathf.Abs((player.position.y - (ceiling - 0.3f) * playerGravityDirection) / playerRb.linearVelocity.y);
            targetPosition = new Vector2(player.position.x + playerRb.linearVelocity.x * time, 4.7f * playerGravityDirection);
        }
        else if(math.dot(playerScript.gravityDirection.y, playerRb.linearVelocity) < -2f && targetPosition.y > -0.6f && playerGravityDirection == -1)
        {
            targetPosition.y = -0.7f;
        }
        else if(math.dot(playerScript.gravityDirection.y, playerRb.linearVelocity) < -2f && targetPosition.y < 0.6f && playerGravityDirection == 1)
        {
            targetPosition.y = 0.7f;
        }
        Vector2 direction = targetPosition - (Vector2)firePoint.position;
        GameObject bullet = Instantiate(bullet1Prefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet1>().ShootBullet(bullet1Speed, direction);
    }


    void ShootBullet2()
    {
        float speed;
        int bulletDirectionX = 1;

        //銃弾の重力がkirikaecounterごとに切り替わるようにするために後から追加してみたやつ
        creatBulletCounter++;
        if(creatBulletCounter > kirikaeCounter)
        {
            creatBulletCounter = 0;
            bulletGravityDirection *= -1;
            // skipCounterは球の重力変更時に球を生成しない回数
            skipCounter = 2;
        }
        if(skipCounter > 0)
        {
            skipCounter--;
            return;
        }

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