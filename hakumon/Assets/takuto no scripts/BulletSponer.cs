using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletSponer : MonoBehaviour
{
    private Vector2 gravityDirection = Vector2.down;
    public Player playerScript;
    public Transform player;
    private float playerGravityDrection;
    private Rigidbody2D rb;
    public GameObject bulletExpPrefab;
    private GameObject bullet;
    public float shootCoolTime = 1f;
    void Start()
    {
        InvokeRepeating(nameof(Shoot), 1f, shootCoolTime);
    }

    void Shoot()
    {
        float differenceX = (player.position.x - transform.position.x);
        float differenceY = math.abs(player.position.y - transform.position.y);
        if(differenceX < 0)
        {
            differenceX *= -1f;
        }
        float speed = differenceX * math.sqrt(10 /math.abs(differenceX - differenceY));
        bullet = Instantiate(bulletExpPrefab, transform.position, quaternion.identity);
        bullet.GetComponent<BulletExp>().Shoot(playerScript.gravityDirection, speed);
    }
    
}
