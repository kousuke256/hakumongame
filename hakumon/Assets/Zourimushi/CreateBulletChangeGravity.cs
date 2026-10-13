using UnityEngine;
using Unity.Mathematics;
using System.Collections;

public class CreateBulletChangeGravity : MonoBehaviour
{
    public float shootStartDelay;
    public float shootCoolTime;
    public Vector2 BulletDirection;
    public float speed;
    public float effectDuration;
    public GameObject BulletChangeGravity;
    public ParticleSystem createEffect;
    private float timer = 0f;
    public Transform player;
    public Player playerScript;


    void Start()
    {
        createEffect.Stop();
        timer = shootCoolTime - shootStartDelay;
    }

    void Update()
    {
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
        bullet.GetComponent<BulletChangeGravity>().Fire(BulletDirection, speed);
        bullet.GetComponent<BulletChangeGravity>().player = player;
        bullet.GetComponent<BulletChangeGravity>().playerScript = playerScript;
    }
}