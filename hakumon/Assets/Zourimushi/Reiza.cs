using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Reiza : MonoBehaviour
{
    public bool existCoolTime;
    public float lifeTime;
    public float firstCoolTime = 0f;
    public float coolTime;
    public bool canInterrupt = false; 
    public float usuReizaDistance = 20f;
    public LayerMask stopLayer;
    public GameObject reizaaa;
    public GameObject usuReizaaa;
    private BoxCollider2D boxCol;
    private bool nowFired = false;
    private float timer;
    private float currentHit;
    private bool canMove;
    private bool first = true;
    private Vector2 firePoint1;
    private Vector2 firePoint2;

    void Start()
    {
        boxCol = reizaaa.GetComponent<BoxCollider2D>();

        firePoint1 = transform.up * 0.5001f + transform.right * -0.01f;
        firePoint2 = transform.up * 0.5001f + transform.right * 0.01f;

        // 初期設定
        if (existCoolTime)
        {
            timer = firstCoolTime;
            return;
        }

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

        timer -= Time.fixedDeltaTime;
        if(timer <= 0f)
        {
            nowFired = !nowFired;
            if (nowFired)
            {
                timer += lifeTime;

                // 発射する状態にする
                first = true;
            }
            else
            {
                timer += coolTime;
                // 発射しない状態にする
                reizaaa.SetActive(false);
            }
        }

        if(!nowFired)
        return;

        Fire();
    }

    void Fire()
    {
        // 発射位置からgroundまでの距離を測る
        RaycastHit2D hit1 = Physics2D.Raycast((Vector2)transform.position + firePoint1, transform.up, 50f, stopLayer);
        RaycastHit2D hit2 = Physics2D.Raycast((Vector2)transform.position + firePoint2, transform.up, 50f, stopLayer);

        // 発射位置からgroundまでの距離が変わっていないかつ、Fireメゾットの2回目以降の実行(cooltimeがあけて)だったらreturn
        if((currentHit == hit1.distance || currentHit == hit2.distance) && !first)
        return;

        // Raycastで当たった距離を決定
        if (hit1.collider == null)
        {
            currentHit = 20f;
        }
        else
        {
            currentHit = hit1.distance;
        }

        if(currentHit < 0.05f)
        {
            reizaaa.SetActive(false);
            return; 
        }

        // 先にレーザーの見た目の大きさを変更
        reizaaa.transform.localScale = new Vector3(1f, currentHit, 1f);

        // レーザーの位置を変更
        reizaaa.transform.localPosition = new Vector2(0f, currentHit / 2f + 0.5f);

        // Colliderの大きさを設定
        boxCol.size = new Vector2(boxCol.size.x, (currentHit - 0.4f) / Mathf.Abs(reizaaa.transform.lossyScale.y));

        reizaaa.SetActive(true);

        if(!canInterrupt)
        return;

        if(currentHit > usuReizaDistance - 1f)
        {
            usuReizaaa.SetActive(false);
            return;
        }

        usuReizaaa.transform.localScale = new Vector3(1f, usuReizaDistance - currentHit, 1f);
        usuReizaaa.transform.localPosition = new Vector2(0f, (usuReizaDistance - currentHit) / 2f + currentHit + 0.5f);
        usuReizaaa.SetActive(true);

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
