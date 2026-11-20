using Unity.VisualScripting;
using UnityEngine;

public class GChanger : MonoBehaviour
{
    [SerializeField] private Vector2 changeGDirection;

    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.gameObject.CompareTag("Player"))
        return;

        Player1 playerScript = collision.GetComponent<Player1>();

        playerScript.gravitychange(changeGDirection.normalized);
        Destroy(gameObject);
    }
}
