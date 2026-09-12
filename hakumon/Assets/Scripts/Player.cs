using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("移動")]
    public float moveSpeed = 4f; //地上での移動速度
    public float airMoveSpeed = 3f; //空中での移動速度
    public float groundAcceleration = 20f;
    public float airAcceleration = 10f;
    [Header("ジャンプ")]
    public float jumpPower = 9.3f;
    public float jumpCutMultiplier = 0.4f; //小ジャンプになるときの挙動の感じを決める変数
    public float miriJumpTime = 0.14f;
    public float smallJumpTime = 0.3f;
    public float mediumJumpTime = 0.45f;
    private float jumpHoldTime = 0f;
    private float jumpCutTime = 100f;
    private float jumpCountUp = 0f;
    private bool jumpPressed;
    private bool canCutJump;
    [Header("設置判定")]
    private bool isGrounded;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private Rigidbody2D playerRb;
    private SpriteRenderer sr;
    private float move;
    private float jumpBufferTime = 0.1f; //space入力の持続時間
    private float jumpBufferCounter;
    public float groundCheckDistance = 0.45f;
    public Vector2 gravityDirection = Vector2.down;
    [Header("重力")]
    public float gravityPower = 9.8f;
   
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        playerRb.gravityScale = 0f;
    }

    void Update()
    {   
        //横移動の方向の取得、変数moveでplayerの進む向きを変えている
        move = 0;
        if (Keyboard.current.dKey.isPressed)
        move = 1;
        if (Keyboard.current.aKey.isPressed)
        move = -1;

        //重力操作
        if(isGrounded)
        {
            if (Keyboard.current.wKey.wasPressedThisFrame)
            {
                gravityDirection = Vector2.up;
                gravitychange();
            }
            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                gravityDirection = Vector2.down;
                gravitychange();
            }
        }

        //ジャンプ判定を記憶
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpBufferCounter = jumpBufferTime; //Spaceが押されてからしばらくの間Spaceが押された判定が続くことによってジャンプしやすくしてる
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        //spaceKeyを押しているかどうか
        if (Keyboard.current.spaceKey.isPressed)
        {
            jumpPressed = true;
        }
        else
        {
            jumpPressed = false;
        }
    }

    void FixedUpdate()
    {
        float targetSpeed;
        float acceleration;

        //接地判定, 下の1行はPlayerの子オブジェクトのGroundCheckの中心から半径0.2以内に、LayerがGroundのオブジェクトがあればisGroundがtrueになるというコード
        isGrounded = Physics2D.OverlapCircle(groundCheck.position,groundCheckRadius,groundLayer);//これは

        // 重力
        playerRb.AddForce(gravityDirection * gravityPower);

        //横移動
        if (isGrounded)
        {
            targetSpeed = move * moveSpeed;
            acceleration = groundAcceleration;
        }
        else
        {
            targetSpeed = move * airMoveSpeed;
            if(move == 0)
            {
                acceleration = 1f;
            }
            else
            {
                acceleration = airAcceleration;
            }
        }

        float newx = Mathf.MoveTowards(playerRb.linearVelocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
        playerRb.linearVelocity = new Vector2(newx, playerRb.linearVelocity.y);

        //ジャンプ
        Jump();

        //小ジャンプ中ジャンプ処理
        CutJump();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Enemyタグにぶつかったとき
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // 接触面の向き
            ContactPoint2D contact = collision.GetContact(0);
            // 敵の上から踏んだ場合
            if (contact.normal.y > 0.5f)
            {
                // 敵を倒す
                Destroy(collision.gameObject);
                // 踏みの反動
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, jumpPower);
            }
            canCutJump = true;
        }
    }

    private void Jump()
    {
        if (jumpBufferCounter > 0 && isGrounded)
        {
            // 現在の速度を取得
            Vector2 velocity = playerRb.linearVelocity;

            // 重力方向の速度を取得
            float gravityVelocity = Vector2.Dot(velocity, gravityDirection);
    
            // 重力方向の速度を取り除く
            velocity -= gravityDirection * gravityVelocity;
    
            // 重力と逆方向にジャンプ速度を追加
            velocity += -gravityDirection * jumpPower;

            // 速度を設定
            playerRb.linearVelocity = velocity;

            jumpCutTime = 100f;
            jumpBufferCounter = 0f;
            canCutJump = true;
        }
    }

    private void CutJump()
    {
        if (canCutJump)
        {
            //小ジャンプ、中ジャンプがおきる時間を決める
            if(jumpCountUp < mediumJumpTime)
            {
                jumpCountUp += Time.fixedDeltaTime;
                
                if (jumpPressed)
                {
                    jumpHoldTime += Time.fixedDeltaTime;
                }
                else
                {
                    if (jumpHoldTime < miriJumpTime)
                    {
                        Debug.Log("ミリジャンプ");
                        jumpCutTime = miriJumpTime;
                    }
                    else if (jumpHoldTime < smallJumpTime)
                    {
                        Debug.Log("小ジャンプ");
                        jumpCutTime = smallJumpTime;
                    }
                    else if (jumpHoldTime < mediumJumpTime)
                    {
                        Debug.Log("中ジャンプ");
                        jumpCutTime = mediumJumpTime;
                    }

                }
            }
            else
            {
                Debug.Log("大ジャンプ");
                canCutJump = false;
                jumpCountUp = 0f;
                jumpHoldTime = 0f;
            }

            //大ジャンプ以外の処理
            if (jumpCountUp > jumpCutTime)
            {
                if (Vector2.Dot(playerRb.linearVelocity, gravityDirection) < 0 )
                {
                    jumpCountUp = 0f;
                    jumpHoldTime = 0f;
                    jumpCutTime = 100f;
                    canCutJump = false;//現在の重力方向への速度をjumpCutMultiplier倍している
                    playerRb.linearVelocity -= gravityDirection * Vector2.Dot(playerRb.linearVelocity, gravityDirection) * (1 - jumpCutMultiplier);
                }
            }
        }
    }

    private void gravitychange()
    {
        sr.flipY = !sr.flipY;
        groundCheck.localPosition = gravityDirection * groundCheckDistance;
        playerRb.AddForce(gravityDirection * 2f, ForceMode2D.Impulse);
    }
}