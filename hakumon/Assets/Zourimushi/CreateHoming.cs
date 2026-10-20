using UnityEngine;
using Unity.Mathematics;
using System.Collections;
using UnityEngine.UIElements;

public class CreateHoming : MonoBehaviour
{
    public float shootStartDelay;
    public float shootCoolTime;
    public float speed = 5f;
    public float lifeTime = 8f;
    public float turnSpeed = 90f;
    public float effectDuration;
    public float scale = 2f;
    public Transform player;
    public Player playerScript;
    public GameObject homingBullet;
    public ParticleSystem createEffect;
    public LayerMask groundLayer;
    private float timer = 0f;
    private bool canMove;


    void Start()
    {
        createEffect.Stop();
        timer = shootCoolTime - shootStartDelay;
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

    void Update()
    {
        if(!canMove)
        return;

        timer += Time.deltaTime;
        if(timer >= shootCoolTime)
        {
            timer -= shootCoolTime;
            StartCoroutine(CreateBullet());
        }
        
    }

    IEnumerator CreateBullet()
    {   
        createEffect.Play();

       //  effectDuration秒待つ
        yield return new WaitForSeconds(effectDuration - 0.5f);
        createEffect.Stop();
        yield return new WaitForSeconds(0.5f);
        createEffect.Clear();

        // 地面がなかったr生成する
        if(!Physics2D.OverlapCircle(transform.position, 0.8f, groundLayer))
        {
            // 弾を生成する
            GameObject bullet = Instantiate(homingBullet, transform.position, Quaternion.identity);
            BulletHoming bulletScript = bullet.GetComponent<BulletHoming>();
            bulletScript.scale = scale;
            bulletScript.speed = speed;
            bulletScript.turnSpeed = turnSpeed;
            bulletScript.lifeTime = lifeTime;
            bulletScript.player = player;
            bulletScript.playerScript = playerScript;
            bulletScript.canFire = true;
        }
    }
}
