using UnityEngine;

public class DashRing : MonoBehaviour

{

    [Header("プレイヤー")]

    [SerializeField] private Player player;

    [Header("リング画像：直接の子オブジェクト")]

    [SerializeField] private SpriteRenderer visual;

    [Header("矢印：リング画像とは別の子オブジェクト")]

    [SerializeField] private Transform arrow;

    [Header("飛ぶ角度：右0、上90、左180、下270")]

    [Range(0f, 360f)]

    public float launchAngle = 45f;

    [Header("飛ぶ速度")]

    [Min(0.1f)]

    public float launchSpeed = 16f;

    [Header("ダッシュを維持する秒数")]

    [Min(0.02f)]

    public float dashDuration = 0.3f;

    [Header("リングの直径")]

    [Min(0.1f)]

    public float ringSize = 2f;

    [Header("再発動までの秒数")]

    [Min(0f)]

    public float cooldown = 0.3f;

    private float previousGravityY;

    private float nextUseTime;

    private void Start()

    {

        if (player == null)

        {

            Debug.LogWarning(

                "DashRingのPlayer欄を設定してください。",

                this

            );

            enabled = false;

            return;

        }

        previousGravityY = player.gravityDirection.y;

        ApplyVisualSize();

        UpdateArrow();

    }

    // PlayerのUpdateで重力が変更された後に確認

    private void LateUpdate()

    {

        UpdateArrow();

        ApplyVisualSize();

        if (player == null)

            return;

        float currentGravityY = player.gravityDirection.y;

        // 重力の上下が入れ替わったか

        bool gravityChanged =

            previousGravityY * currentGravityY < 0f;

        // 範囲外でも履歴は更新する

        previousGravityY = currentGravityY;

        if (Time.timeScale == 0f)

            return;

        if (!gravityChanged || player.isGrounded)

            return;

        if (Time.time < nextUseTime)

            return;

        // プレイヤーの中心がリング内にあるか

        Vector2 difference =

            (Vector2)player.transform.position

            - (Vector2)transform.position;

        float radius = ringSize * 0.5f;

        if (difference.sqrMagnitude > radius * radius)

            return;

        Vector2 direction = GetLaunchDirection();

        // 指定したプレイヤーを飛ばす

        player.StartRingDash(

            direction,

            launchSpeed,

            dashDuration

        );

        nextUseTime = Time.time + cooldown;

    }

    private Vector2 GetLaunchDirection()

    {

        // 角度をラジアンへ変換

        float radians = launchAngle * Mathf.Deg2Rad;

        return new Vector2(

            Mathf.Cos(radians),

            Mathf.Sin(radians)

        );

    }

    private void UpdateArrow()

    {

        if (arrow == null)

            return;

        // 右向きの矢印画像を飛ぶ方向へ回転

        arrow.rotation = Quaternion.Euler(

            0f,

            0f,

            launchAngle

        );

    }

    private void ApplyVisualSize()

    {

        if (visual == null || visual.sprite == null)

            return;

        // リング画像は、このオブジェクトの直接の子にする

        if (visual.transform.parent != transform)

            return;

        Vector2 spriteSize = visual.sprite.bounds.size;

        if (spriteSize.x <= 0f || spriteSize.y <= 0f)

            return;

        visual.transform.localScale = new Vector3(

            ringSize / spriteSize.x,

            ringSize / spriteSize.y,

            1f

        );

    }

    private void OnValidate()

    {

        ringSize = Mathf.Max(0.1f, ringSize);

        launchSpeed = Mathf.Max(0.1f, launchSpeed);

        dashDuration = Mathf.Max(0.02f, dashDuration);

        cooldown = Mathf.Max(0f, cooldown);

        ApplyVisualSize();

        UpdateArrow();

    }

    private void OnDrawGizmosSelected()

    {

        Vector3 center = transform.position;

        Vector3 direction = GetLaunchDirection();

        // 発動範囲

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(

            center,

            ringSize * 0.5f

        );

        // Scene上で飛ぶ方向を表示

        Gizmos.color = Color.yellow;

        float arrowLength = Mathf.Max(1f, ringSize);

        Vector3 tip = center + direction * arrowLength;

        Gizmos.DrawLine(center, tip);

        Vector3 side = new Vector3(

            -direction.y,

            direction.x,

            0f

        );

        Gizmos.DrawLine(

            tip,

            tip - direction * 0.3f + side * 0.2f

        );

        Gizmos.DrawLine(

            tip,

            tip - direction * 0.3f - side * 0.2f

        );

    }

}
