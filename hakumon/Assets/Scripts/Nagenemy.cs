using System.Collections;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

public class Nagenemy : MonoBehaviour
{
    [Header("弾の情報")]
    public float shootStartDelay;
    public float shootCoolTime;
    public float bulletMaxSpeed = 20f;
    public int bulletGravityDirection;
    public float ceiling;
    public Transform player;
    public Transform firePoint;
    public GameObject bullet2Prefab;

    [Header("見た目")]
    public Sprite normalSprite;
    public Sprite AttackSprite;

    [Header("弾を持つ位置")]
    public Vector3 holdOffset = new Vector3(-0.5f, 0.5f, 0f);

    private bool canMove;
    private float y = 1;
    private bool isDead = false;

    private SpriteRenderer sr;

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
        bullet2Prefab,
        transform.position + offset,
        Quaternion.identity
    );

    SpriteRenderer bulletSr =
        bullet.GetComponent<SpriteRenderer>();

    Vector3 originalScale = bullet.transform.localScale;

    if (bulletSr != null)
    {
        Color c = bulletSr.color;
        c.a = 0f;
        bulletSr.color = c;
    }

    bullet.transform.localScale = originalScale * 0.8f;

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

        float t = timer / chargeTime;

        if (bulletSr != null)
        {
            Color c = bulletSr.color;
            c.a = t;
            bulletSr.color = c;
        }

        bullet.transform.localScale =
            Vector3.Lerp(
                originalScale * 0.8f,
                originalScale,
                t
            );

        timer += Time.deltaTime;

        yield return null;
    }

    if (bulletSr != null)
    {
        Color c = bulletSr.color;
        c.a = 1f;
        bulletSr.color = c;
    }

    bullet.transform.localScale = originalScale;

    float speed;
    int bulletDirectionX = 1;

    float differenceX = player.position.x - firePoint.position.x;
    float differenceY = bulletGravityDirection * (player.position.y - firePoint.position.y);

    if (differenceX < 0)
    {
        differenceX *= -1f;
        bulletDirectionX = -1;
    }

    float theta = Mathf.Atan(
        (differenceY + Mathf.Sqrt(
            differenceX * differenceX +
            differenceY * differenceY))
        / differenceX
    );

    float tan = math.tan(theta);

    float maxHeight =
        differenceX * differenceX * tan * tan
        / (4 * (differenceX * tan - differenceY));

    while (firePoint.position.y * bulletGravityDirection + maxHeight > ceiling)
    {
        theta -= Mathf.Deg2Rad * 3f;

        tan = math.tan(theta);

        maxHeight =
            differenceX * differenceX * tan * tan
            / (4 * (differenceX * tan - differenceY));
    }

    speed =
        differenceX / math.cos(theta)
        * math.sqrt(
            5 / math.abs(differenceX * tan - differenceY)
        );

    speed = bulletMaxSpeed < speed
        ? bulletMaxSpeed
        : speed;

    bullet.transform.position = firePoint.position;

    bullet.GetComponent<Bullet2>().Shoot(
        bulletDirectionX,
        bulletGravityDirection,
        speed,
        theta
    );

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
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            return;
        }
    }
}

 