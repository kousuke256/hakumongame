using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float airMoveSpeed = 3f;
    public float jumpPower = 8f;
    public float jumpCutMultiplier = 0.4f;
    private bool jumpPressed;
    private bool jumpCut;
    private bool isGrounded;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private Rigidbody2D playerRb;
    private float move;
    private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;
   
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {   
        //横移動
        move = 0;
        if (Keyboard.current.dKey.isPressed)
        move = 1;
        if (Keyboard.current.aKey.isPressed)
        move = -1;

        //接地判定
        isGrounded = Physics2D.OverlapCircle(groundCheck.position,groundCheckRadius,groundLayer);

        //ジャンプ判定を記憶
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        //spaceKeyを押しているかどうか
        if (Keyboard.current.spaceKey.isPressed)
        {
            jumpPressed = true;
        }
        else
        {
            jumpPressed = false;
        }
    }

    void FixedUpdate()
    {
        //横移動
        playerRb.linearVelocity = new Vector2(move * (isGrounded ? moveSpeed : airMoveSpeed), playerRb.linearVelocity.y);

        //ジャンプ
        if (jumpBufferCounter > 0 && isGrounded)
        {
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x,jumpPower);
            jumpBufferCounter = 0;
            jumpCut = true;
        }

        //小ジャンプにする
        if (playerRb.linearVelocity.y > 0 && !jumpPressed && jumpCut)
        {
            jumpCut = false;
            playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, playerRb.linearVelocity.y * jumpCutMultiplier);
        }
    }
    /*private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;
    
        Gizmos.color = Color.red;
    
        Gizmos.DrawWireSphere(groundCheck.position,groundCheckRadius);
    }*/
}