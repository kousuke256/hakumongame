using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class GravitySniperBullet : MonoBehaviour

{

    [Header("ビームの外側と芯")]

    public LineRenderer beamLine;

    public LineRenderer coreLine;

    [Header("ビームの色")]

    public Color beamColor = Color.cyan;

    public Color coreColor = Color.white;

    [Header("ビームの太さと長さ")]

    [Min(0.01f)] public float beamWidth = 0.18f;

    [Min(0.01f)] public float beamLength = 2.5f;

    [Header("先端の当たり判定の半径")]

    [Min(0.01f)] public float hitRadius = 0.09f;

    [Header("ビームが消えるまでの秒数")]

    [Min(0.1f)] public float lifeTime = 3f;

    [Header("ビームを止める壁・床のレイヤー")]

    public LayerMask groundLayers;

    private Vector2 direction;

    private Vector2 shotOrigin;

    private float speed;

    private Player targetPlayer;

    void Awake()

    {

        // 外側の線は、空欄ならビーム本体から取得する

        if (beamLine == null)

            beamLine = GetComponent<LineRenderer>();

    }

    public void Shoot(Vector2 shootDirection, float shootSpeed, Player player)

    {

        direction = shootDirection.normalized;

        speed = shootSpeed;

        targetPlayer = player;

        shotOrigin = transform.position;

        SetupLine(beamLine, beamWidth, beamColor);

        if (coreLine != null)

        {

            SetupLine(coreLine, beamWidth / 3f, coreColor);

            // 白い芯を水色の線より手前に表示する

            coreLine.sortingLayerID = beamLine.sortingLayerID;

            coreLine.sortingOrder = beamLine.sortingOrder + 1;

        }

        Destroy(gameObject, lifeTime);

    }

    void Update()

    {

        float moveDistance = speed * Time.deltaTime;

        if (moveDistance <= 0f) return;

        Vector2 start = transform.position;

        // 今いる位置から、次のフレームの位置までをまとめて判定する

        RaycastHit2D[] hits = Physics2D.CircleCastAll(

            start, hitRadius, direction, moveDistance

        );

        foreach (RaycastHit2D hit in hits)

        {

            Player hitPlayer = hit.collider.GetComponentInParent<Player>();

            if (hitPlayer != null && hitPlayer == targetPlayer)

            {

                Destroy(gameObject);

                hitPlayer.Deth();

                return;

            }

            // プレイヤーより手前に壁があれば、そこで消える

            int layer = hit.collider.gameObject.layer;

            bool isWall = (groundLayers.value & (1 << layer)) != 0;

            if (isWall && !hit.collider.isTrigger)

            {

                Destroy(gameObject);

                return;

            }

        }

        Vector2 tip = start + direction * moveDistance;

        transform.position = tip;

        DrawBeam(tip);

    }

    private void SetupLine(LineRenderer line, float width, Color color)

    {

        line.useWorldSpace = true;

        line.loop = false;

        line.positionCount = 2;

        line.startWidth = line.endWidth = width;

        line.startColor = line.endColor = color;

        // 発射直後は両端とも銃口にある

        line.SetPosition(0, shotOrigin);

        line.SetPosition(1, shotOrigin);

    }

    private void DrawBeam(Vector2 tip)

    {

        float traveled = Vector2.Distance(shotOrigin, tip);

        float length = Mathf.Min(traveled, beamLength);

        Vector2 tail = tip - direction * length;

        // 水色の線と白い芯を描く

        beamLine.SetPosition(0, tail);

        beamLine.SetPosition(1, tip);

        if (coreLine != null)

        {

            coreLine.SetPosition(0, tail);

            coreLine.SetPosition(1, tip);

        }

        // ゲートと同じように、光っている線に沿って細長い判定を置く

        Vector2 center = (tail + tip) / 2f;

        Vector2 size = new Vector2(length, beamWidth);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, angle);

        foreach (Collider2D hit in hits)

        {

            Player hitPlayer = hit.GetComponentInParent<Player>();

            if (hitPlayer != null && hitPlayer == targetPlayer)

            {

                Destroy(gameObject);

                hitPlayer.Deth();

                return;

            }

        }

    }


}
