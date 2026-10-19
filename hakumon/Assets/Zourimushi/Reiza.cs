using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Reiza : MonoBehaviour
{
    public bool existCoolTime;
    public float lifeTime;
    public float coolTime;
    public LayerMask stopLayer;
    public GameObject reizaaa;
    private BoxCollider2D boxCol;
    private bool nowFired = false;
    private float timer;
    private float currentHit;
    private bool canMove;
    private float positionX;
    private bool first = true;
    private Vector2 firePoint1;
    private Vector2 firePoint2;

    void Start()
    {
        boxCol = reizaaa.GetComponent<BoxCollider2D>();

        firePoint1 = transform.up * 0.5001f + transform.right * -0.01f;
        firePoint2 = transform.up * 0.5001f + transform.right * 0.01f;

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
        RaycastHit2D hit1 = Physics2D.Raycast((Vector2)transform.position + firePoint1, transform.up, 30f, stopLayer);
        RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position + firePoint2, transform.up, 30f, stopLayer);

        // 発射位置からgroundまでの距離が変わっていないかつ、Fireメゾットの2回目以降の実行(cooltimeがあけて)だったらreturn
        if((currentHit == hit1.distance || currentHit == hit2.distance) && !first)
        return;

        if(hit1.collider == null)
        {
            currentHit = 16f;
            boxCol.size = new Vector2(boxCol.size.x, 1f);
        }
        else if (hit1.distance <= 1f)
        {
            boxCol.size = new Vector2(boxCol.size.x, 0.5f);
            currentHit = hit1.distance;
        }
        else
        {
            currentHit = hit1.distance;
            boxCol.size = new Vector2(boxCol.size.x, 1f);
        }
        first = false;

        Vector2 a = (Vector2)transform.position + firePoint1;
        Vector2 b = (Vector2)transform.position + firePoint2;
        
        // レーザーｎo大きさを変えru
        reizaaa.transform.localScale = new Vector3(1f, currentHit - 0.01f, 1f);
        reizaaa.transform.localPosition = new Vector2(0f, currentHit / 2f + 0.5f);

        reizaaa.SetActive(true);
    }

    /*if(hit1.collider == null && hit2.collider == null)
        {
            currentHit = 16f;
            boxCol.size = new Vector2(boxCol.size.x, 1f);
        }
        else if(hit1.collider == null || hit2.collider == null)
        {
            currentHit = Mathf.Max(hit1.distance, hit2.distance);
            boxCol.size = new Vector2(boxCol.size.x, 1f);
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
