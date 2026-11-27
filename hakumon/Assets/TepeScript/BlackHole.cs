using UnityEngine;

public class BlackHole : MonoBehaviour

{

    [Header("プレイヤー")]

    [SerializeField] private Player player;

    [Header("吸引する重力方向")]

    [Tooltip("ON：上向きで吸引、OFF：下向きで吸引")]

    public bool attractWhenGravityUp = false;

    [Header("見た目")]

    [SerializeField] private SpriteRenderer visual;

    public Color pullColor = new Color(0.7f, 0.3f, 1f);

    public Color pushColor = new Color(0.2f, 1f, 1f);

    [Header("効果範囲の半径")]

    [Min(0.1f)]

    public float pullRadius = 4f;

    [Header("吸引の強さ")]

    [Min(0f)]

    public float pullPower = 25f;

    [Header("反発の強さ")]

    [Min(0f)]

    public float pushPower = 40f;

    [Header("中心の死亡範囲")]

    [Min(0.01f)]

    public float deathRadius = 0.4f;

    private Rigidbody2D playerRb;

    private bool swallowed;

    private void Start()

    {

        if (player == null)

        {

            Debug.LogWarning(

                "BlackHoleのPlayer欄を設定してください。",

                this

            );

            enabled = false;

            return;

        }

        playerRb = player.GetComponent<Rigidbody2D>();

        if (playerRb == null)

        {

            Debug.LogWarning(

                "PlayerにRigidbody2Dが必要です。",

                this

            );

            enabled = false;

            return;

        }

        // 未設定なら同じオブジェクトから取得

        if (visual == null)

        {

            visual = GetComponent<SpriteRenderer>();

        }

        UpdateColor();

    }

    private void Update()

    {

        if (player == null)

            return;

        UpdateColor();

    }

    private void UpdateColor()

    {

        if (visual == null)

            return;

        bool gravityUp = player.gravityDirection.y > 0f;

        // 現在の重力と、吸引する設定が一致すれば吸引

        bool isAttracting =

            gravityUp == attractWhenGravityUp;

        visual.color = isAttracting

            ? pullColor

            : pushColor;

    }

    private void FixedUpdate()

    {

        if (swallowed || player == null || playerRb == null)

            return;

        // プレイヤーから中心へ向かう矢印

        Vector2 difference =

            (Vector2)transform.position - playerRb.position;

        float distance = difference.magnitude;

        // 中心は吸引・反発どちらの状態でも死亡

        if (distance <= deathRadius)

        {

            swallowed = true;

            player.Deth();

            return;

        }

        // 効果範囲の外なら何もしない

        if (distance >= pullRadius)

            return;

        // 外側で0、中心付近で1

        float closeness = Mathf.InverseLerp(

            pullRadius,

            deathRadius,

            distance

        );

        bool gravityUp = player.gravityDirection.y > 0f;

        // 吸引する設定と一致しなければ反発

        bool isRepelling =

            gravityUp != attractWhenGravityUp;

        Vector2 direction;

        float power;

        if (isRepelling)

        {

            // 外側へ押し出す

            direction = -difference.normalized;

            power = pushPower;

        }

        else

        {

            // 中心へ吸い込む

            direction = difference.normalized;

            power = pullPower;

        }

        playerRb.AddForce(

            direction * power * closeness * playerRb.mass,

            ForceMode2D.Force

        );

    }

    private void OnValidate()

    {

        pullRadius = Mathf.Max(0.1f, pullRadius);

        deathRadius = Mathf.Clamp(

            deathRadius,

            0.01f,

            pullRadius - 0.01f

        );

        pullPower = Mathf.Max(0f, pullPower);

        pushPower = Mathf.Max(0f, pushPower);

    }

    private void OnDrawGizmosSelected()

    {

        // 水色：吸引・反発の範囲

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(

            transform.position,

            pullRadius

        );

        // 赤色：死亡する範囲

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(

            transform.position,

            deathRadius

        );

    }

}
