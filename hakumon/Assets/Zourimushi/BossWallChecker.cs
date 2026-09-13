using UnityEngine;

public class BossWallChecker : MonoBehaviour
{
    public GameObject boss;
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Ground"))
        {
            gameObject.SetActive(false);
            Invoke(nameof(InactiveWallchecker), 0.8f);
            boss.GetComponent<Boss>().JumpOrTurn();
        }
    }

    void InactiveWallchecker()
    {
        gameObject.SetActive(true);
    }
    
}
