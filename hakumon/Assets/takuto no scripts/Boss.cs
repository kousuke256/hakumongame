using Unity.Collections;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.InputSystem;

public class Boss : MonoBehaviour
{
    public Transform player;
    public GameObject bulletPrefab;
    public Transform firePoint;
    private float moveDirection;
    public float speed = 2.3f;
    private bool canWalk = true;
    public float jumpPower = 5.4f;
    public int maxTrun = 3;
    private int turnCount = 0;
    public float gravityPower = 9.8f;
    public  float shootCoolTime = 0.6f;
    private Vector2 gravityDirection = Vector2.right;

    private Rigidbody2D rb;

    int[] directions =
    {
        1,
        -1,
    };

    public enum enemystate
    {
        walk,
        JumpAtack,
        ShootAtack,
        GravityAtack,
        Freeze,

    }

    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        UpdateRotation();
        moveDirection = directions[Random.Range(0, directions.Length)];
        Debug.Log(gravityDirection);
        InvokeRepeating(nameof(Shoot), 1f, shootCoolTime);
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            gravityDirection = Vector2.left;
        }
        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            gravityDirection = Vector2.right;
        }
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            gravityDirection = Vector2.up;
        }
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            gravityDirection = Vector2.down;
        }*/
    }

    void FixedUpdate()
    {
        //Debug.Log(moveDirection);
        rb.AddForce(gravityDirection * gravityPower);
        if (canWalk)
        {
            Walk();
        }
    }

    void SetState()
    {
        
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = player.position - firePoint.position;
        bullet.GetComponent<Bullet>().ShootBullet(direction);
    }

    void GravityChenge()
    {
        gravityDirection = new Vector2(-moveDirection * gravityDirection.y, moveDirection * gravityDirection.x);
        Debug.Log(gravityDirection);
        UpdateRotation();
    }
     void UpdateRotation()
    {
        Vector2 upDirection = -gravityDirection;
        transform.up = upDirection;
    }

    void Walk()
    {
        Vector2 velocity = rb.linearVelocity;
        Vector2 moveVelocity = transform.right * moveDirection * speed;
        float gravitySpeed = Vector2.Dot(velocity, gravityDirection);
        Vector2 gravityVelosity = gravitySpeed * gravityDirection;
        rb.linearVelocity = gravityVelosity + moveVelocity;
        
    }

    public void JumpOrTurn()
    {
        if(Random.Range(0, 2) == 0 || turnCount == maxTrun)
        {
            turnCount = 0;
            rb.linearVelocity = transform.up * jumpPower;
            Invoke(nameof(GravityChenge), 0.4f);
        }
        else
        {
            moveDirection *= -1;
            turnCount++;
        }
    }

    void CantWalk()
    {
        canWalk = false;
    }
}