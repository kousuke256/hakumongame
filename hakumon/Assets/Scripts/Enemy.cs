using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField,Header("移動速度")]
    private float moveSpeed;

    private Rigidbody2D rigid;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        //rigid.velocity = new Vector2(-moveSpeed,rigid.velocity.y);
        rigid.linearVelocity = new Vector2(-moveSpeed, rigid.linearVelocity.y);
    }
}
