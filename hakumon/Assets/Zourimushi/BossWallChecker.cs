using UnityEngine;

public class BossWallChecker : MonoBehaviour
{
    public Boss bossScript;
    void OnTriggerStay2D(Collider2D collider)
    {
        if(!bossScript.isGrounded)
        return;

        if (!collider.gameObject.CompareTag("Ground"))
        return;

        gameObject.SetActive(false);
        Invoke(nameof(InactiveWallchecker), 0.7f);
        bossScript.JumpOrTurn();
    }

    void InactiveWallchecker()
    {
        gameObject.SetActive(true);
    }
    
}
