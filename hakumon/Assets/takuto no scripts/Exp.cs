using UnityEngine;

public class Exp : MonoBehaviour
{
    public float shootPower = 8f;
    private Rigidbody2D rb;
    public float gravityPower = 9.8f;
    private Vector2 gravityDirection = Vector2.down;
    void Start()
    {
        rb =GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(shootPower, shootPower * 1.5f);
    }

    void FixedUpdate()
    {
        rb.AddForce(gravityDirection * gravityPower);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
