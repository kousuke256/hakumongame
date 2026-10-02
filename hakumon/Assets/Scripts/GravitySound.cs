using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    public AudioClip GravitySound;
    AudioSource audioSource;

    void Start()
    {
        //Componentを取得
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Debug.Log(gameObject.name);
        // 左
        if (Input.GetKey(KeyCode.W))
        {
            //音(sound1)を鳴らす
            audioSource.PlayOneShot(GravitySound);
        }
    }
}