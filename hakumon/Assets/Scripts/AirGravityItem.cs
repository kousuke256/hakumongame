using UnityEngine;

public class AirGravityItem : MonoBehaviour

{

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)

    {

        if (collected) return;

        Player player = other.GetComponentInParent<Player>();

        if (player == null) return;

        // すでに所持している場合は、アイテムを残す

        if (!player.TryGetAirGravityItem())

            return;

        collected = true;

        Destroy(gameObject);

    }

}
