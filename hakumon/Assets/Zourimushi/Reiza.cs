using UnityEngine;

public class Reiza : MonoBehaviour
{
    public float lifeTime;
    public float coolTime;
    private bool nowFired;
    private float timer;

    void Start()
    {
        
    }

    void Update()
    {
        timer += Time.deltaTime;
        // レーザー発射中ならlifeTimeのカウントをしていて、発射していないならcoolTimeのカウントをしている
        if (nowFired)
        {
            Fire();


            if(timer < lifeTime)
            return;
            
            timer -= lifeTime;
            nowFired = false;
        }
        else
        {
            if(timer < coolTime)
            return;
            
            timer -= coolTime;
            nowFired = true;
        }
    }

    void Fire()
    {
        
    }
}
