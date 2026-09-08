using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 4f; //地上での移動速度
    public float airMoveSpeed = 3f; //空中での移動速度
    public float groundAcceleration = 15f;
    public float airAcceleration = 20f;
    public float jumpPower = 8f;
    public float jumpCutMultiplier = 0.4f; //小ジャンプになるときの挙動の感じを決める変数
    private bool jumpPressed;
    private bool canCutJump;
    private bool isGrounded;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private Rigidbody2D playerRb;
    private SpriteRenderer sr;
    private float move;
    private float jumpBufferTime = 0.1f; //space入力の持続時間
    private float jumpBufferCounter;
    private float gravitySign = 1f;
    private Vector3 groundCheckOriginalPosition;
   
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        groundCheckOriginalPosition = groundCheck.localPosition;
    }

    void Update()
    {   
        //横移動の方向の取得、変数moveでplayerの進む向きを変えている
        move = 0;
        if (Keyboard.current.dKey.isPressed)
        move = 1;
        if (Keyboard.current.aKey.isPressed)
        move = -1;


        //接地判定, 下の1行はPlayerの子オブジェクトのGroundCheckの中心から半径0.2以内に、LayerがGroundのオブジェクトがあればisGroundがtrueになるというコード
        isGrounded = Physics2D.OverlapCircle(groundCheck.position,groundCheckRadius,groundLayer);//これは


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
        //横移動
        float newx = Mathf.MoveTowards(playerRb.linearVelocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
        playerRb.linearVelocity = new Vector2(newx, playerRb.linearVelocity.y);


        //ジャンプ
        if (jumpBufferCounter > 0 && isGrounded)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x,jumpPower * gravitySign);
            jumpBufferCounter = 0;
            canCutJump = true; //敵キャラを踏むときにも変数canCutJump=trueの文をいれる感じにしたい
        }


        //spaceをジャンプの上昇中に離すと小ジャンプにする
        if (playerRb.linearVelocity.y * gravitySign > 0 && !jumpPressed && canCutJump)
        {
            canCutJump = false;
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, playerRb.linearVelocity.y * jumpCutMultiplier);//y軸方向の速さをjumpCutMultiplier倍している
        }
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
                playerRb.linearVelocity =
                    new Vector2(playerRb.linearVelocity.x, jumpPower);
                // Spaceを離したとき小ジャンプ
                canCutJump = true;
            }
        }

        //重力反転ボタンを押したとき
        if (collision.gameObject.CompareTag("gravityButtom"))
        {
            gravitySign *= -1;
            playerRb.gravityScale *= -1;
            sr.flipY = !sr.flipY;
            groundCheck.localPosition = new Vector3(groundCheck.localPosition.x, gravitySign * groundCheckOriginalPosition.y, groundCheck.localPosition.z);
        }
    }

    /*private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;
    
        Gizmos.color = Color.red;
    
        Gizmos.DrawWireSphere(groundCheck.position,groundCheckRadius);
    }*/
}