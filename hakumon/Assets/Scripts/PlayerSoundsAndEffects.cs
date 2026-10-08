using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSoundsAndEffects : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip GravitySound;
    private Vector2 oldPlayerGravitydirection = Vector2.down;
    public ParticleSystem gravityEffect;
    public Player playerScript;
    public Camera mainCamera;
    private static PlayerSoundsAndEffects instance;
    void Awake()
    {
        //  シングルトンというらしい、わからなかったらぐぐれ
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (playerScript == null)
        return;
        
        // 重力が変わったら
        if(oldPlayerGravitydirection != playerScript.gravityDirection)
        {

            oldPlayerGravitydirection = playerScript.gravityDirection;
            audioSource.PlayOneShot(GravitySound);
            
            // 重力が左右方向に変わったらreturnさせる
            if(oldPlayerGravitydirection.y == 0)
            return;

            PlayGravityEffect();
        }
    }

    void PlayGravityEffect()
    {
        Vector3 effectPosition;
        Vector3 cameraPosition = mainCamera.transform.position;
        float cameraHeight = mainCamera.orthographicSize * 2f;

        // 重力方向によって出す場所を変更
        if (oldPlayerGravitydirection == Vector2.down)
        {
            // 画面上端
            effectPosition = cameraPosition + new Vector3(-10.5f, cameraHeight / 2f, 15);
        }
        else
        {
            // 画面下端
            effectPosition = cameraPosition + new Vector3(-10.5f, -cameraHeight / 2f, 15);
        }
        // 横幅28マスからエフェクトを出す(横幅7のParticleSystemを4か所から出しているから)
        // エフェクトがなるべく均等にかつ、ランダムに出るようにこのプログラムを実装してみた
        for(int i = 1; i <= 4; i++)
        {
            ParticleSystem effect = Instantiate(gravityEffect, effectPosition , Quaternion.identity);
            effect.transform.up = oldPlayerGravitydirection;
            effect.Play();
            Destroy(effect.gameObject, 1.5f);
            effectPosition.x += 7f; 
        }
        
        
        //StartCoroutine(FadeOutEffect(effect));

    }

    /*IEnumerator FadeOutEffect(ParticleSystem effect)
    {
        // 2秒待つ
        yield return new WaitForSeconds(0.2f);

        ParticleSystemRenderer renderer =effect.GetComponent<ParticleSystemRenderer>();
        Material material = renderer.material;
        Color color = material.color;
        float startAlpha = color.a;

        // 1秒かけて透明にする
        float fadeTime = 1f;
        float timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(startAlpha, 0f, timer / fadeTime);

            color.a = alpha;
            material.color = color;

            yield return null;
        }
        // 完全に透明にする
        color.a = 0f;
        material.color = color;

        Destroy(effect.gameObject);
    }*/
}