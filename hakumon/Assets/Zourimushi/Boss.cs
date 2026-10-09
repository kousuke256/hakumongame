

using System;
using System.Runtime.InteropServices;
using NUnit.Framework.Internal.Commands;
using Unity.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Boss : MonoBehaviour
{
    public bossState state;
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
    private bool wasGrounded;
    private bool land = false;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bossState previousState = bossState.Start;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool chaseAction = false;
    private float oldPlayerGravityDirection = 0f;
    private float oldVelocityX = 0f;

    public enum bossState
    {
        Start,
        walk,
        chase,
        chaseStun,
        ShootAtack1,
        ShootAtack2,
        GravityAtack,

    }

    // paternChangeTimeで次の行動が何秒続くかを決め、stateを変えている
    void SetState()
    {
        switch (state)
        {
            case bossState.Start : 
                patternChangeTime = 2f;
                state = bossState.walk;
                break;

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
        }
    }

    void Start()
    {
        gravityDirection = Vector2.right;
        rb = GetComponent<Rigidbody2D>();
        playerRb = player.GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
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

        if(state == bossState.Start)
        return;

        if(state == bossState.chaseStun)
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
        
        // 着地した瞬間かどうかを判断する
        land = false;
        if(wasGrounded != isGrounded)
        {
            land = true;
        }
        wasGrounded = isGrounded;
        

        // 重力が左右だったら
        if (gravityDirection.y == 0)
        {
            // moveDirectionの方向に慣性のある移動をする
            float newY = Mathf.MoveTowards(rb.linearVelocity.y, moveDirection * chaseSpeed * transform.right.y, chaseAcceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, newY);

            // 落下攻撃を可能にする
            canChaseJump = true;
            chaseAction = false;

            if(!isGrounded)
            return;

            if(land)
            return;

            // bossが張り付いている壁とplayerの距離が近いのにもかかわらずbossが高いところにいたらジャンプできないようにするif文
            float wallPlayerDistance = 10 - gravityDirection.x * (player.position.x + playerRb.linearVelocity.x);
            float groundBossDistance = 5 - gravityDirection.x * moveDirection * transform.position.y;
            if(wallPlayerDistance - groundBossDistance < 0f)
            return;

            if(groundBossDistance > 7f)
            return;

            float jumpPower = chaseJumpPower * (groundBossDistance + 5f) / 8.5f;
            if (math.abs(player.position.y - transform.position.y) < 5f && playerScript.gravityDirection.y * oldPlayerGravityDirection > 0 && wallPlayerDistance - groundBossDistance > 5f)
            {
                jumpPower += 0.5f;
                float time = wallPlayerDistance / jumpPower;
                float power = (5f * time) - (playerScript.gravityDirection.y * (player.position.y - transform.position.y) / time);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, power * -playerScript.gravityDirection.y);
                Jump(jumpPower);
                // 重力の方向がプレイヤーと同じに
                moveDirection *= -1;
                gravityDirection = playerScript.gravityDirection;
                transform.up = -gravityDirection;
                return;
            }

            // bossが高いところからジャンプするほどジャンプ力が高くなる
            // groundBossDistanceに5を足しているのはどれだけ地面からの距離が近くてもある程度ジャンプさせるため
            Jump(jumpPower);
            GravityChengeLeftOrRight();
        }
        else
        {
            // 重力が上下
            if (!isGrounded)
            return;

            if (land)
            {
                //　着地した瞬間、進んでいる方向にplayerがいたら
                if(rb.linearVelocity.x * (player.position.x - transform.position.x) > 0 && playerScript.gravityDirection.y == gravityDirection.y)
                chaseAction = true;
                oldVelocityX = rb.linearVelocity.x;
            }

            if((canChaseJump && playerScript.gravityDirection.y != gravityDirection.y) || chaseAction)
            {
                // 慣性のある移動をしながらプレイヤーを追う
                int wowowoMoveDirection = player.position.x + playerRb.linearVelocity.x * 0.5f > transform.position.x ? 1 : -1;
                float newX = Mathf.MoveTowards(rb.linearVelocity.x, wowowoMoveDirection * chaseSpeed, chaseAcceleration * Time.fixedDeltaTime);
                rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
                moveDirection = gravityDirection.y == -1 ? wowowoMoveDirection : -wowowoMoveDirection;

                if(rb.linearVelocity.x * oldVelocityX < 0)
                chaseAction = false;

                oldVelocityX = rb.linearVelocity.x;

                if(!canChaseJump)
                return;

                if(gravityDirection == playerScript.gravityDirection)
                return;

                // playerが重力を変更した直後にbossが落下攻撃するのを防ぐための処理
                // playerと地面との距離が5超過の場合returnを実行してジャンプできないようにしている
                if(5 - playerScript.gravityDirection.y * player.position.y > 5f)
                return;

                // 0.7fはBossの落下時間, playerの位置を予測してbossが天井から落ちてくるプログラム
                float playerFuturePosiition = player.position.x + playerRb.linearVelocity.x * 0.5f;
                float bossFuturePosiition = transform.position.x + rb.linearVelocity.x * 0.8f;
                if(Mathf.Abs(playerFuturePosiition - bossFuturePosiition) < 1f)
                {
                    // 落下攻撃をする
                    Jump(chaseJumpPower);
                    gravityDirection *= -1;
                    transform.up = -gravityDirection;
                    moveDirection *= -1;
                    canChaseJump = false;
                }
            }
            else
            {
                // 慣性なしの移動、変数moveDirectionによって進む向きが決まる
                float newX = Mathf.MoveTowards(rb.linearVelocity.x, moveDirection * chaseSpeed * transform.right.x, chaseAcceleration * Time.fixedDeltaTime);
                rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);

                // 進んでいる方向にある壁からの距離がcanより大きかった場合return;
                float wallBossDistance = 10 + transform.position.x * moveDirection * gravityDirection.y;
                if(wallBossDistance > 7f)
                return;

                // playerと地面との距離が5超過の場合returnを実行してジャンプできないようにしている
                if(5 - playerScript.gravityDirection.y * player.position.y > 5f)
                return;

                // 進んでいる方向にある壁からの距離に比例してジャンプ力が高くなる
                // wallBossDistanceに1を足しているのはどれだけ壁からの距離が近くてもある程度ジャンプさせるため
                Jump(chaseJumpPower * (wallBossDistance + 1f) / 4f);
                GravityChengeLeftOrRight();
                oldPlayerGravityDirection = playerScript.gravityDirection.y;
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

        float theta = Mathf.Atan((differenceY + Mathf.Sqrt(differenceX * differenceX + differenceY * differenceY)) / differenceX);
        float tan = math.tan(theta);
        float maxHeight = differenceX * differenceX * tan * tan / (4 * (differenceX * tan - differenceY));
        while(firePoint.position.y * bulletGravityDirection + maxHeight > ceiling)
        {
            theta -= Mathf.Deg2Rad * 3f;
            tan = math.tan(theta);
            maxHeight = differenceX * differenceX * tan * tan / (4 * (differenceX * tan - differenceY));
        }
        speed = (differenceX / math.cos(theta) * math.sqrt(5 / math.abs(differenceX * tan - differenceY)));
        speed = bulletMaxSpeed < speed ? bulletMaxSpeed : speed;

        GameObject bullet = Instantiate(bullet2Prefab, firePoint.position, quaternion.identity);
        bullet.GetComponent<Bullet2>().Shoot(bulletDirectionX, bulletGravityDirection, speed, theta);
    }
}