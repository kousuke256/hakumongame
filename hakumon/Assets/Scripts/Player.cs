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

    public bool canmove = true;

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

    public float groundCheckDistance = 0.6f;

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

        // 横移動の入力

        move = 0f;

        if (Keyboard.current.dKey.isPressed)

        {

            move = 1f;

            sr.flipX = false;

        }

        if (Keyboard.current.aKey.isPressed)

        {

            move = -1f;

            sr.flipX = true;

        }

        // 重力操作

        if (isGrounded || AirGravityChange)

        {

            Vector2 nextGravity = gravityDirection;

            if (Keyboard.current.wKey.wasPressedThisFrame)

            {

                nextGravity = Vector2.up;

            }

            else if (Keyboard.current.sKey.wasPressedThisFrame)

            {

                nextGravity = Vector2.down;

            }

            // 今と違う向きに変更するときだけ実行

            if (nextGravity != gravityDirection)

            {

                // 空中で使ったときだけアイテムを消費

                if (!isGrounded)

                {

                    AirGravityChange = false;

                }

                gravityDirection = nextGravity;

                sr.flipY = gravityDirection.y > 0f;

                // 前のジャンプ処理をリセット

                canCutJump = false;

                jumpCountUp = 0f;

                jumpHoldTime = 0f;

                jumpCutTime = 100f;

                gravitychange();

                // 重力変更前の接地判定を解除

                isGrounded = false;

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

            ringDashTime = Mathf.Max(

                0f,

                ringDashTime - Time.fixedDeltaTime

            );

            isGrounded = false;

            jumpBufferCounter = 0f;

            playerRb.linearVelocity = ringDashVelocity;

            // この下の重力・移動・ジャンプ処理を実行しない

            return;

        }

        float targetSpeed;

        float acceleration;

        // 足元にある床を取得

        Collider2D ground = Physics2D.OverlapBox(

            groundCheck.position,

            new Vector2(0.575f, 0.1f),

            0f,

            groundLayer

        );

        isGrounded = ground != null;

        // Iceタグの床なら滑る

        onSlipperyFloor =

            ground != null && ground.CompareTag("Ice");

        // 重力

        playerRb.AddForce(

            gravityDirection * gravityPower * playerRb.mass

        );

        // 横移動

        if (isGrounded)

        {

            targetSpeed = move * moveSpeed;

            if (onSlipperyFloor)

            {

                if (move == 0f)

                {

                    // 氷の上でキーを離したとき

                    acceleration = slipperyDeceleration;

                }

                else

                {

                    // 氷の上で加速・切り返し

                    acceleration = slipperyAcceleration;

                }

            }

            else

            {

                acceleration = groundAcceleration;

            }

        }

        else

        {

            targetSpeed = move * airMoveSpeed;

            if (move == 0f)

            {

                acceleration = 1f;

            }

            else

            {

                acceleration = airAcceleration;

            }

        }

        float newx = Mathf.MoveTowards(

            playerRb.linearVelocity.x,

            targetSpeed,

            acceleration * Time.fixedDeltaTime

        );

        playerRb.linearVelocity = new Vector2(

            newx,

            playerRb.linearVelocity.y

        );

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

    public void gravitychange()

    {

        if (gravityDirection.y == 1)

        {

            groundCheck.localPosition = new Vector3(

                groundCheck.localPosition.x,

                0.6f,

                groundCheck.localPosition.z

            );

        }

        else

        {

            groundCheck.localPosition = new Vector3(

                groundCheck.localPosition.x,

                -0.6f,

                groundCheck.localPosition.z

            );

        }

        playerRb.AddForce(

            gravityDirection * 2f * playerRb.mass,

            ForceMode2D.Impulse

        );

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
