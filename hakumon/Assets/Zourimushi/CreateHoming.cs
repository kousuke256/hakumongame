using UnityEngine;
using Unity.Mathematics;
using System.Collections;

public class CreateHoming : MonoBehaviour
{
    public float shootStartDelay;
    public float shootCoolTime;
    public float speed = 5f;
    public float lifeTime = 8f;
    public float turnSpeed = 90f;
    public float effectDuration;
    public Transform player;
    public Player playerScript;
    public GameObject homingBullet;
    public ParticleSystem createEffect;
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

        // 弾を生成する
        GameObject bullet = Instantiate(homingBullet, transform.position, Quaternion.identity);
        bullet.GetComponent<BulletHoming>().speed = speed;
        bullet.GetComponent<BulletHoming>().turnSpeed = turnSpeed;
        bullet.GetComponent<BulletHoming>().lifeTime = lifeTime;
        bullet.GetComponent<BulletHoming>().player = player;
        bullet.GetComponent<BulletHoming>().playerScript = playerScript;
    }
}
