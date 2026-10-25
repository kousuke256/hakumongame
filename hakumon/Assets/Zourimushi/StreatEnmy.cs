using System.Collections;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

public class StreatEnmy : MonoBehaviour
{
    [Header("弾の情報")]
    public float shootStartDelay;
    public float shootCoolTime;
    public float bullet1Speed = 13f;
    public float ceiling = 7f;
    public Transform firePoint;
    public GameObject bullet1Prefab;
    public Transform player;
    public Rigidbody2D playerRb;
    public Player playerScript;

    [Header("見た目")]
    public Sprite normalSprite;
    public Sprite AttackSprite;

    [Header("弾を持つ位置")]
    public Vector3 holdOffset = new Vector3(-0.5f, 0.5f, 0f);

    private SpriteRenderer sr;

    private bool canMove;
    private float y = 1;
    private bool isDead = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        InvokeRepeating(nameof(Throw), shootStartDelay, shootCoolTime);

        if (GetComponent<Rigidbody2D>().gravityScale < 0)
        {
            y = -1;
        }
    }

    void Update()
    {
        if (!canMove)
            return;

        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1, y, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, y, 1);
        }
    }

    void OnBecameVisible()
    {
        canMove = true;
    }

    void OnBecameInvisible()
    {
        canMove = false;
    }

    void Throw()
    {
        if (!canMove)
            return;

        StartCoroutine(ThrowRoutine());
    }

    IEnumerator ThrowRoutine()
{
    sr.sprite = AttackSprite;

    Vector3 offset = holdOffset;

    if (transform.localScale.x < 0)
    {
        offset.x *= -1f;
    }

    GameObject bullet = Instantiate(
        bullet1Prefab,
        transform.position + offset,
        Quaternion.identity
    );

    float chargeTime = 1f;
    float timer = 0f;

    while (timer < chargeTime)
    {
        offset = holdOffset;

        if (transform.localScale.x < 0)
        {
            offset.x *= -1f;
        }

        bullet.transform.position =
            transform.position + offset;

        timer += Time.deltaTime;

        yield return null;
    }

    Vector2 distance = player.position - firePoint.position;
    Vector2 playerVelocity = playerRb.linearVelocity;

    float A = playerVelocity.sqrMagnitude - bullet1Speed * bullet1Speed;
    float B = Vector2.Dot(distance, playerVelocity);
    float C = distance.sqrMagnitude;

    float discriminant = B * B - A * C;

    if (discriminant < 0)
    {
        Destroy(bullet);
        sr.sprite = normalSprite;
        yield break;
    }

    float sqrtDiscriminant = Mathf.Sqrt(discriminant);

    float time = (-B + sqrtDiscriminant) / A;

    if (time < 0f)
        time = (-B - sqrtDiscriminant) / A;

    Vector2 targetPosition =
        (Vector2)player.position + playerVelocity * time;

    float playerGravityDirection =
        playerScript.gravityDirection.y;

    if (targetPosition.y * playerGravityDirection > ceiling)
    {
        time = Mathf.Abs(
            (
                player.position.y
                - (ceiling - 0.3f) * playerGravityDirection
            )
            / playerRb.linearVelocity.y
        );

        targetPosition = new Vector2(
            player.position.x +
            playerRb.linearVelocity.x * time,
            4.7f * playerGravityDirection
        );
    }
    else if (
        math.dot(
            playerScript.gravityDirection.y,
            playerRb.linearVelocity
        ) < -2f
        && targetPosition.y > -0.6f
        && playerGravityDirection == -1
    )
    {
        targetPosition.y = -0.7f;
    }
    else if (
        math.dot(
            playerScript.gravityDirection.y,
            playerRb.linearVelocity
        ) < -2f
        && targetPosition.y < 0.6f
        && playerGravityDirection == 1
    )
    {
        targetPosition.y = 0.7f;
    }

    Vector2 direction =
        targetPosition - (Vector2)firePoint.position;

    bullet.transform.position = firePoint.position;

    bullet.GetComponent<Bullet1>()
        .ShootBullet(bullet1Speed, direction);

    sr.sprite = normalSprite;
}

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (y == 1)
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    if (contact.normal.y < -0.5f)
                    {
                        return;
                    }
                }
            }
            else
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {
                    if (contact.normal.y > 0.5f)
                    {
                        return;
                    }
                }
            }

            if (isDead)
                return;

            isDead = true;

            if (LifeManager.Instance == null)
            {
                Debug.LogError("LifeManagerがありません！");
                return;
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

            return;
        }
    }
}