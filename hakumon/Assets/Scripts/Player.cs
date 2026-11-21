using UnityEngine;

using UnityEngine.InputSystem;

using UnityEngine.SceneManagement;

public class Player : MonoBehaviour

{

    private Rigidbody2D playerRb;

    private SpriteRenderer sr;

    [Header("移動")]

    public float moveSpeed = 4f;

    public float airMoveSpeed = 3f;

    public float groundAcceleration = 20f;

    public float airAcceleration = 10f;

    private float move;

    [Header("ジャンプ")]

    public float jumpPower = 9.3f;

    public float jumpCutMultiplier = 0.4f;

    public float smallJumpTime = 0.3f;

    public float mediumJumpTime = 0.45f;

    private float jumpHoldTime = 0f;

    private float jumpCutTime = 100f;

    private float jumpCountUp = 0f;

    public bool jumpPressed;

    private bool CanJump = true;

    private bool canCutJump;

    private float jumpBufferTime = 0.1f;

    private float jumpBufferCounter;

    [Header("接地判定")]

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

    private bool isDead = false;

    private AudioSource audiosource;

    public AudioClip JumpSound;

    // 【追加】リングによるダッシュ

    private float ringDashTime;

    private Vector2 ringDashVelocity;

    public bool TryGetAirGravityItem()

    {

        // すでに持っているなら拾わない

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

        ShowPauseMenu.showPauseMenu = true;

        LifeManager manager = LifeManager.Instance;

        if (manager != null
&& manager.hasSavePoint
&& manager.savedSceneName

                == SceneManager.GetActiveScene().name)

        {

            transform.position = manager.respawnPosition;

        }

    }

    void Update()

    {

        if (Time.timeScale == 0f)

            return;

        //横移動の方向の取得、変数moveでplayerの進む向きを変えている
        move = 0;

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

        // 重力操作
        if (isGrounded || AirGravityChange)
        {
            if (Keyboard.current.shiftKey.wasPressedThisFrame)
            {
                // 空中で使ったときだけアイテムを消費
                if (!isGrounded)
                {
                    AirGravityChange = false;
                }

                gravitychange(-gravityDirection);

            }
        }

        // ジャンプ入力を短時間記憶

        if (Keyboard.current.spaceKey.wasPressedThisFrame)

        {

            jumpBufferCounter = jumpBufferTime;

        }

        else

        {

            jumpBufferCounter -= Time.deltaTime;

        }

        jumpPressed =

            Keyboard.current.spaceKey.isPressed && CanJump;

        // 画面外で死亡

        if (gravityDirection.y == -1
&& transform.position.y < -8)

        {

            Deth();

        }

        if (gravityDirection.y == 1
&& transform.position.y > 8)

        {

            Deth();

        }

    }

    // 【追加】DashRingから呼ばれる

    public void StartRingDash(

        Vector2 direction,

        float speed,

        float duration

    )

    {

        if (isDead || playerRb == null)

            return;

        ringDashVelocity = direction.normalized * speed;

        ringDashTime = duration;

        // ジャンプの減速処理がダッシュに干渉しないようにする

        canCutJump = false;

        jumpCountUp = 0f;

        jumpHoldTime = 0f;

        jumpCutTime = 100f;

        jumpBufferCounter = 0f;

        isGrounded = false;

        playerRb.linearVelocity = ringDashVelocity;

    }

    void FixedUpdate()

    {

        // 【追加】ダッシュ中は通常の移動処理を休む

        if (ringDashTime > 0f)

        {

            ringDashTime = Mathf.Max(0f, ringDashTime - Time.fixedDeltaTime);

            isGrounded = false;

            jumpBufferCounter = 0f;

            playerRb.linearVelocity = ringDashVelocity;

            // この下の重力・移動・ジャンプ処理を実行しない

            return;

        }


        // 足元にある床を取得

        Collider2D ground = Physics2D.OverlapBox(groundCheck.position, new Vector2(0.575f, 0.1f), 0f, groundLayer);

        isGrounded = ground != null;

        // Iceタグの床なら滑る

        onSlipperyFloor = ground != null && ground.CompareTag("Ice");

        // 重力

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

        float targetSpeed = move * moveSpeed;

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

        Jump();

        CutJump();

    }

    private void OnCollisionEnter2D(Collision2D collision)

    {

        if (collision.gameObject.CompareTag("Enemy"))

        {

            ContactPoint2D contact = collision.GetContact(0);

            if (gravityDirection.y == -1)

            {

                if (contact.normal.y > 0.5f)

                {

                    Destroy(collision.gameObject);

                    playerRb.linearVelocity = new Vector2(

                        playerRb.linearVelocity.x,

                        jumpPower

                    );

                }

                canCutJump = true;

            }

            else

            {

                if (contact.normal.y < -0.5f)

                {

                    Destroy(collision.gameObject);

                    playerRb.linearVelocity = new Vector2(

                        playerRb.linearVelocity.x,

                        -jumpPower

                    );

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

            Vector2 velocity = playerRb.linearVelocity;

            // 重力方向の速度を取得

            float gravityVelocity = Vector2.Dot(

                velocity,

                gravityDirection

            );

            // 重力方向の速度を取り除く

            velocity -= gravityDirection * gravityVelocity;

            // 重力と逆方向へジャンプ

            velocity += -gravityDirection * jumpPower;

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

        // 小ジャンプ・中ジャンプのタイミングを決める

        if (jumpCountUp < mediumJumpTime)

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

        // 大ジャンプ以外の減速

        if (jumpCountUp > jumpCutTime)

        {

            if (Vector2.Dot(

                    playerRb.linearVelocity,

                    gravityDirection

                ) < 0)

            {

                jumpCountUp = 0f;

                jumpHoldTime = 0f;

                jumpCutTime = 100f;

                canCutJump = false;

                playerRb.linearVelocity -=

                    gravityDirection

                    * Vector2.Dot(

                        playerRb.linearVelocity,

                        gravityDirection

                    )

                    * (1 - jumpCutMultiplier);

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

            CanJump = false;

        }

        else

        {

            CanJump = true;

        }

    }

    public void Deth()

    {

        if (isDead)

            return;

        isDead = true;

        AudioClip deathSound =

            Resources.Load<AudioClip>("DeathSound");

        if (deathSound != null)

        {

            GameObject soundObject =

                new GameObject("DeathSound");

            DontDestroyOnLoad(soundObject);

            AudioSource source =

                soundObject.AddComponent<AudioSource>();

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

            SceneManager.LoadScene(

                SceneManager.GetActiveScene().buildIndex

            );

        }

    }

    private void OnDrawGizmos()

    {

        if (groundCheck == null)

            return;

        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(

            groundCheck.position,

            new Vector3(0.575f, 0.1f, 0f)

        );

    }

}
