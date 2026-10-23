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
    private bool canMove;
    private float y = 1;
    private bool isDead =false;


    void Start()
    {   
        // shootStartDelay秒後に弾を投げ始めて、shootCoolTime秒間隔で投げる
        InvokeRepeating(nameof(Throw), shootStartDelay, shootCoolTime);

        if(GetComponent<Rigidbody2D>().gravityScale < 0)
        {
            y=-1;
        }
    }

    void Update()
    {
        if(!canMove)
        return;

        if (player.position.x > transform.position.x)
        {
            // Playerが右にいる場合は右を向く
            transform.localScale = new Vector3(-1, y, 1);
        }
        else
        {
            // Playerが左にいる場合は左を向く
            transform.localScale = new Vector3(1, y, 1);
        }
    }

    // 画面内にこいつがいる場合は動作を可能にして、画面外にいる場合は動作を不能にする
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
        if(!canMove)
        return;

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

    void OnCollisionEnter2D(Collision2D collision)
    {
        // プレイヤーとの接触
        if (collision.gameObject.CompareTag("Player"))
        {
            // 上から踏まれた場合
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
            if(isDead)
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
