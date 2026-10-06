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

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
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
        // カメラの中心
        Vector3 cameraPosition = mainCamera.transform.position;
        // カメラの高さ
        float cameraHeight = mainCamera.orthographicSize * 2f;

        // 重力方向によって出す場所を変更
        if (oldPlayerGravitydirection == Vector2.down)
        {
            // 画面上端
            effectPosition = cameraPosition + new Vector3(0, cameraHeight / 2f, 0);
        }
        else
        {
            // 画面下端
            effectPosition = cameraPosition + new Vector3(0, -cameraHeight / 2f, 0);
        }
        ParticleSystem effect = Instantiate(gravityEffect, effectPosition, Quaternion.identity);
        effect.transform.up = oldPlayerGravitydirection;
        effect.Play();
        Destroy(effect.gameObject, 1.5f);
    }
}