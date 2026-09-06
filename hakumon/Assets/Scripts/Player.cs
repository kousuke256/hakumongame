using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 4f; //地上での移動速度
    public float airMoveSpeed = 3f; //空中での移動速度
    public float jumpPower = 8f;
    public float jumpCutMultiplier = 0.4f; //小ジャンプになるときの挙動の感じを決める変数
    private bool jumpPressed;
    private bool canCutJump;
    private bool isGrounded;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private Rigidbody2D playerRb;
    private float move;
    private float jumpBufferTime = 0.1f; //space入力の持続時間
    private float jumpBufferCounter;
   
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
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
        //横移動
        playerRb.linearVelocity = new Vector2(move * (isGrounded ? moveSpeed : airMoveSpeed), playerRb.linearVelocity.y);


        //ジャンプ
        if (jumpBufferCounter > 0 && isGrounded)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x,jumpPower);
            jumpBufferCounter = 0;
            canCutJump = true; //敵キャラを踏むときにも変数canCutJump=trueの文をいれる感じにしたい
        }


        //spaceをジャンプの上昇中に離すと小ジャンプにする
        if (playerRb.linearVelocity.y > 0 && !jumpPressed && canCutJump)
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
    }
    /*private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;
    
        Gizmos.color = Color.red;
    
        Gizmos.DrawWireSphere(groundCheck.position,groundCheckRadius);
    }*/
}