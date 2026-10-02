using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerSoundsAndEffects : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip GravitySound;
    private Vector2 currentPlayerGravitydirection = Vector2.down;
    public Player playerScript;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 重力が変わったら
        if(currentPlayerGravitydirection != playerScript.gravityDirection)
        {
            currentPlayerGravitydirection = playerScript.gravityDirection;
            
            GravityEffect(currentPlayerGravitydirection);

            audioSource.PlayOneShot(GravitySound);
        }
    }

    void GravityEffect(Vector2 gravityDirection)
    {
        
    }
    
}