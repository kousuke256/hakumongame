using UnityEngine;
using Unity.Mathematics;
using System.Collections;

public class CreateBulletChangeGravity : MonoBehaviour
{
    public float shootStartDelay;
    public float shootCoolTime;
    public Vector2 BulletDirection;
    public float speed;
    public float lifeTime = 8f;
    public float effectDuration;
    public float scale = 2f;
    public bool canMoveWhenInvisible = false;
    public GameObject BulletChangeGravity;
    public ParticleSystem createEffect;
    private float timer = 0f;
    private bool canMove;
    public Transform player;
    public Player playerScript;


    void Start()
    {
        createEffect.Stop();
        timer = shootCoolTime - shootStartDelay;
        if (canMoveWhenInvisible)
        {
            canMove = true;
        }
        else
        {
            canMove = false;
        }
    }

    // 画面内にこいつがいる場合は動作を可能にして、画面外にいる場合は動作を不能にする
    void OnBecameVisible()
    {
        if(canMoveWhenInvisible)
        return;

        canMove = true;
    }
    void OnBecameInvisible()
    {
        if(canMoveWhenInvisible)
        return;
        
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

        // 弾を生成する
        GameObject bullet = Instantiate(BulletChangeGravity, transform.position, Quaternion.identity);
        bullet.GetComponent<BulletChangeGravity>().Fire(BulletDirection, speed, scale);
        bullet.GetComponent<BulletChangeGravity>().lifeTime = lifeTime;
        bullet.GetComponent<BulletChangeGravity>().player = player;
        bullet.GetComponent<BulletChangeGravity>().playerScript = playerScript;
    }
}