using UnityEngine;

public class PlayerSoundsAndEffects : MonoBehaviour
{
    private Vector2 currentPlayerGravitydirection = Vector2.down;
    public Player playerScript;


    void Update()
    {
        // 重力が変わったら
        if(currentPlayerGravitydirection != playerScript.gravityDirection)
        {
            currentPlayerGravitydirection = playerScript.gravityDirection;
            GravityEffect(currentPlayerGravitydirection);
        }
    }

    void GravityEffect(Vector2 gravityDirection)
    {
        
    }
    
}