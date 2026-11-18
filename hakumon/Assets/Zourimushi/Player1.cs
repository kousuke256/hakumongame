using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Player1 : MonoBehaviour
{
    public bool pra;
    private Rigidbody2D playerRb;
    private SpriteRenderer sr;
    
    [Header("移動")]
    public float moveSpeed = 4f;
    public float groundAcceleration = 18f;
    public float airAcceleration = 8f;
    private float move;

    public bool canmove=true;

    [Header("ジャンプ")]
    public float jumpPower = 8.2f;
    public float jumpCutMultiplier = 0.5f; //小ジャンプになるときの挙動の感じを決める変数
    public float smallJumpTime = 0.13f;
    public float mediumJumpTime = 0.32f;
    private float jumpHoldTime = 0f;
    private float jumpCutTime = 100f;
    private float jumpCountUp = 0f;
    public bool jumpPressed;
    private bool CanJump=true;
    private bool canCutJump;
    private float jumpBufferTime = 0.1f; // space入力の持続時間
    private float jumpBufferCounter;

    [Header("設置判定")]
    public bool isGrounded;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    public LayerMask groundLayer;
    
    [Header("重力")]
    public float gravityPower = 9.8f;
    public Vector2 gravityDirection = Vector2.down;

    [Header("空中重力変換アイテム")]
    public bool AirGravityChange = false;

    [Header("滑る床")]

    public float slipperyAcceleration = 3f;
    public float slipperyDeceleration = 0.5f;
    private bool onSlipperyFloor;


    // groundCheckのlocalPositionをいじる変数。いつか重力の向きが変わったらスクリプトの向きを変えるプログラムを作るから将来なくなる変数
    public float groundCheckDistance = 0.6f;
    private bool isDead = false;
   
    private AudioSource audiosource;
    public AudioClip JumpSound;

    public bool TryGetAirGravityItem()
    {
        //すでに持っているなら拾わない
        if (AirGravityChange)
            return false;

        AirGravityChange = true;
        return true;
    }
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        playerRb.gravityScale = 0f;
        audiosource = GetComponent<AudioSource>();
        // pauseMenuをひょuじ可能にする
        ShowPauseMenu.showPauseMenu = true;

        LifeManager manager = LifeManager.Instance;
        if (manager != null
        && manager.hasSavePoint
        && manager.savedSceneName ==
              UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
        {
            transform.position = manager.respawnPosition;
        }
    }

    void Update()
    {   
        if(Time.timeScale == 0f)
        return;

        //横移動の方向の取得、変数moveでplayerの進む向きを変えている
        move = 0;

        //
        if(pra){//

        if (Keyboard.current.dKey.isPressed)
        {
            move++;
            sr.flipX = false ;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            move--;
            sr.flipX = true ;
        }

        //
        }else{//
        //

        if(gravityDirection.x == 0)
        {
            if (Keyboard.current.dKey.isPressed)
            {
                move++;
                sr.flipX = false ;
            }

            if (Keyboard.current.aKey.isPressed)
            {
                move--;
                sr.flipX = true ;
            }
        }
        else
        {
            if (Keyboard.current.wKey.isPressed)
            {
                move++;
                sr.flipX = false ;
            }

            if (Keyboard.current.sKey.isPressed)
            {
                move--;
                sr.flipX = true ;
            }
        }

        }//
        //

        // 重力操作
        if (isGrounded || AirGravityChange)
        {
            //
            if(pra){//

            if (Keyboard.current.shiftKey.wasPressedThisFrame)
            {
                // 空中で使ったときだけアイテムを消費
                if (!isGrounded)
                {
                    AirGravityChange = false;
                }

                gravitychange(-gravityDirection);

            }

            //
            }else{//
            //

            Vector2 nextGDirection = new Vector2(0,0);
            if(gravityDirection.x == 0)
            {

                if (Keyboard.current.wKey.wasPressedThisFrame)
                {
                    nextGDirection = Vector2.up;
                }
                else if (Keyboard.current.sKey.wasPressedThisFrame)
                {
                    nextGDirection = Vector2.down;
                }
            }
            else
            {
                if (Keyboard.current.aKey.wasPressedThisFrame)
                {
                    nextGDirection = Vector2.left;
                }
                else if (Keyboard.current.dKey.wasPressedThisFrame)
                {
                    nextGDirection = Vector2.right;
                }
            }

            if(gravityDirection == -nextGDirection)
            {
                gravitychange(nextGDirection);
            }

            }//
            //

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
        if (Keyboard.current.spaceKey.isPressed&&CanJump)
        {
            jumpPressed = true;
        }
        else
        {
            jumpPressed = false;
        }

        if (gravityDirection.y == -1 && transform.position.y<-8)
        {
            Deth();
        }
        if (gravityDirection.y == 1 && transform.position.y>8)
        {
            Deth();
        }
    }

    void FixedUpdate()
    {
        //接地判定, 下の1行はPlayerの子オブジェクトのGroundCheckの中心から半径0.2以内に、LayerがGroundのオブジェクトがあればisGroundがtrueになるというコード
        // 足元にある床を取得
        Collider2D ground = Physics2D.OverlapBox(groundCheck.position, new Vector2(0.575f, 0.1f), 0f, groundLayer);

        // 接地しているか
        isGrounded = ground != null;

        // Iceタグの床なら滑る
        onSlipperyFloor = ground != null && ground.CompareTag("Ice");
        //canmove = Physics2D.OverlapCircle(transform.position,0.2f,groundLayer);

        //重力をかける
        playerRb.AddForce(gravityDirection * gravityPower * playerRb.mass);
        

        /* 加速度を決める */
        float acceleration;
        if (isGrounded)
        {
            if (onSlipperyFloor)
            {
                if (move == 0f)
                {
                    // キーを離したとき：ゆっくり止まる
                    acceleration = slipperyDeceleration;
                }
                else
                {
                    // キーを押したとき：ゆっくり加速・切り返し
                    acceleration = slipperyAcceleration;
                }
            }
            else
            {
                // 普通の床
                acceleration = groundAcceleration;
            }
        }
        else
        {
            // 空中の移動
            if (move == 0f)
            {
                acceleration = 1f;
            }
            else
            {
                acceleration = airAcceleration;
            }
        }  


        /* 移動 */

        float targetSpeed;
        if (pra)
        {
            targetSpeed = gravityDirection.x == -1? -move * moveSpeed : move * moveSpeed;
        }
        else
        {
            targetSpeed = move * moveSpeed;
        }


        if(gravityDirection.x == 0)
        {
            float newx = Mathf.MoveTowards(playerRb.linearVelocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
            playerRb.linearVelocity = new Vector2(newx, playerRb.linearVelocity.y);
        }
        else
        {
            float newY = Mathf.MoveTowards(playerRb.linearVelocity.y, targetSpeed, acceleration * Time.fixedDeltaTime);
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, newY);
        }


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
            if(gravityDirection.y == -1)
            {
                if (contact.normal.y > 0.5f)
                {
                    // 敵を倒す
                    Destroy(collision.gameObject);
                    // 踏みの反動
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, jumpPower);
                }
                canCutJump = true;
            }
            else
            {
                if (contact.normal.y < -0.5f)
                {
                    // 敵を倒す
                    Destroy(collision.gameObject);
                    // 踏みの反動
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, -jumpPower);
                }
                canCutJump = true;
            }

            
            
        }
        if (collision.collider.CompareTag("Deth"))
            {
                Deth();
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
            audiosource.PlayOneShot(JumpSound);
        }
    }

    private void CutJump()
    {
        if (!canCutJump)
        return;

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
                if (jumpHoldTime < smallJumpTime)
                {
                    jumpCutTime = smallJumpTime;
                }
                else if (jumpHoldTime < mediumJumpTime)
                {
                    jumpCutTime = mediumJumpTime;
                }

            }
        }
        else
        {
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

    public void gravitychange(Vector2 nextGravityDirection)
    {
        // 前のジャンプの処理をリセット
        canCutJump = false;
        jumpCountUp = 0f;
        jumpHoldTime = 0f;
        jumpCutTime = 100f;

        gravityDirection = nextGravityDirection;
        transform.up = -gravityDirection;

        // 重力変更直後の古い接地判定を解除
        isGrounded = false;

        if(!isGrounded)
        return;

        playerRb.AddForce(gravityDirection * 2f*playerRb.mass, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Bullet"))
        {
            Deth();
        }
        if (collider.CompareTag("Cloud"))
        {
            CanJump=false;
        }
        else
        {
            CanJump=true;
        }
        return;

        
    }

    void Deth()
    {
        if(isDead)
        return;
        
        isDead = true;
        AudioClip deathSound = Resources.Load<AudioClip>("DeathSound");
        if (deathSound != null)
        {
            GameObject soundObject = new GameObject("DeathSound");
            DontDestroyOnLoad(soundObject);
            AudioSource source = soundObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.clip = deathSound;
            source.volume = 0.7f;
            source.spatialBlend = 0f;
            source.loop = false;
            source.Play();
            Destroy(soundObject, deathSound.length + 0.1f);
        }
        LifeManager.Instance.LoseLife();

            if (LifeManager.Instance.lifeCount <= 0)
            {
                SceneManager.LoadScene("GameOver");
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
    }
}