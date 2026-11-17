using UnityEngine;

public class Trampoline : MonoBehaviour

{

    [Header("落下速度に掛ける倍率")]

    [SerializeField] private float bounceMultiplier = 1.1f;

    [Header("最低・最高の跳ね返り速度")]

    [SerializeField] private float minBounceSpeed = 4f;

    [SerializeField] private float maxBounceSpeed = 18f;

    private void OnCollisionEnter2D(Collision2D collision)

    {

        Player player = collision.gameObject.GetComponentInParent<Player>();

        if (player == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb == null) return;

        Vector2 gravity = player.gravityDirection.normalized;

        if (gravity == Vector2.zero) return;

        // 重力方向に落ちて、乗れる面に当たったか確認する

        // 横からぶつかった場合は跳ねない

        bool hitSurface = false;

        for (int i = 0; i < collision.contactCount; i++)

        {

            ContactPoint2D contact = collision.GetContact(i);

            // トランポリン側で受け取る接触面の向き

            if (Vector2.Dot(contact.normal, gravity) > 0.5f)

            {

                hitSurface = true;

                break;

            }

        }

        if (!hitSurface) return;

        // 衝突時の、重力方向の速度を取り出す

        float fallSpeed = Mathf.Abs(

            Vector2.Dot(collision.relativeVelocity, gravity)

        );

        // 落下が速いほど強く跳ねる

        float bounceSpeed = Mathf.Clamp(

            fallSpeed * bounceMultiplier,

            minBounceSpeed,

            maxBounceSpeed

        );

        // 横方向の移動は残す

        Vector2 velocity = rb.linearVelocity;

        velocity -= gravity * Vector2.Dot(velocity, gravity);

        // 重力と反対向きに跳ね返す

        rb.linearVelocity = velocity - gravity * bounceSpeed;

    }

}
