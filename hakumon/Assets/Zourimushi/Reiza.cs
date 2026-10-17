using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Reiza : MonoBehaviour
{
    public bool existCoolTime;
    public float lifeTime;
    public float coolTime;
    public LayerMask stopLayer;
    public GameObject reizaaa;
    private bool nowFired = false;
    private float timer;
    private float currentHit;
    private bool canMove;
    private bool first = true;
    private float positionX;

    void Start()
    {
        // 初期設定
        if(existCoolTime)
        return;

        reizaaa.SetActive(true);
    }

    void FixedUpdate()
    {
        if (!existCoolTime)
        {
            Fire();
            return;
        }

        //if(!canMove)
        //return;

        timer += Time.fixedDeltaTime;
        // レーザー発射中ならlifeTimeのカウントをしていて、発射していないならcoolTimeのカウントをしている
        if (nowFired)
        {
            Fire();

            if(timer < lifeTime)
            return;
            timer -= lifeTime;

            // 発射しない状態にする
            nowFired = false;
            reizaaa.SetActive(false);
        }
        else
        {
            if(timer < coolTime)
            return;
            timer -= coolTime;

            // 発射する状態にする
            nowFired = true;
            first = true;
        }
    }

    void Fire()
    {
        // 発射位置からgroundまでの距離を測る
        RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.up * 0.51f, transform.up, 30f, stopLayer);

        // 発射位置からgroundまでの距離が変わっていないかつ、Fireメゾットの2回目以降の実行(cooltimeがあけて)だったらreturn
        if(currentHit == hit.distance && !first)
        return;

        if(hit.collider == null)
        {
            currentHit = 20f;
        }
        else
        {
            currentHit = hit.distance;
        }
        first = false;
        
        // レーザーｎo大きさを変えru
        reizaaa.transform.localScale = new Vector3(1f, currentHit, 1f);
        reizaaa.transform.localPosition = new Vector2(0f, currentHit / 2f + 0.5f);

        reizaaa.SetActive(true);
    }

    /*void OnBecameVisible()
    {
        canMove = true;
    }
    void OnBecameInvisible()
    {
        canMove = false;
    }*/
}
