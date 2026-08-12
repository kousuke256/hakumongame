using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]private float moveSpeed=5.0f;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Walk();
    }

    private void Walk()
    {
        float direction = Input.GetAxisRaw("Horizontal");

        rb.linearVelocityX=direction*moveSpeed;
    }
}
