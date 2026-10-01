using UnityEngine;

public class BossWallChecker : MonoBehaviour
{
    public Boss bossScript;
    void OnTriggerEnter2D(Collider2D collider)
    {
        if(!bossScript.isGrounded)
        return;

        if (!collider.gameObject.CompareTag("Ground"))
        return;

        gameObject.SetActive(false);
        Invoke(nameof(InactiveWallchecker), 0.1f);
        bossScript.JumpOrTurn();
    }

    void InactiveWallchecker()
    {
        gameObject.SetActive(true);
    }
    
}
