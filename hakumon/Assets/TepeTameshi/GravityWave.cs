using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class GravityWave : MonoBehaviour

{

    [Header("波の動き")]

    [Min(0.1f)] public float speed = 8f;

    [Min(0.1f)] public float maxRadius = 16f;

    [Header("波の見た目")]

    [Min(8)] public int pointCount = 64;

    [Min(0.01f)] public float width = 0.12f;

    public Color waveColor = Color.cyan;

    public Color coreColor=Color.white;

    private Player player;

    private LineRenderer line;

    public LineRenderer coreLine;

    private float radius;

    private float previousDistance;

    private bool hasHit;

    public void SetPlayer(Player target)

    {

        player = target;

        previousDistance = Vector2.Distance(

            transform.position, player.transform.position

        );

    }

    void Awake()

    {

        line = GetComponent<LineRenderer>();

        line.useWorldSpace = false; // 波自身の位置を円の中心にする

        line.loop = true;

        line.positionCount = pointCount;

        line.startWidth = line.endWidth = width;

        line.startColor = line.endColor = waveColor;

        coreLine.useWorldSpace = false;

        coreLine.loop = true;

        coreLine.positionCount = pointCount;

        coreLine.startWidth = coreLine.endWidth = width / 5f;

        coreLine.startColor = coreLine.endColor = coreColor;

        // 外側の輪より手前に白い芯を描く

        coreLine.sortingLayerID = line.sortingLayerID;

        coreLine.sortingOrder = line.sortingOrder + 1;


        DrawCircle();

    }

    void Update()

    {

        float oldRadius = radius;

        radius = Mathf.Min(radius + speed * Time.deltaTime, maxRadius);

        DrawCircle();

        if (player != null && !hasHit)

        {

            float distance = Vector2.Distance(

                transform.position, player.transform.position

            );

            // 正ならプレイヤーはフチの外側、負なら内側

            float before = previousDistance - oldRadius;

            float now = distance - radius;

            float halfWidth = width * 0.5f;

            // フチに重なったか、前のフレームからフチをまたいだか

            if (Mathf.Min(before, now) <= halfWidth &&

                Mathf.Max(before, now) >= -halfWidth)

            {

                hasHit = true;

                player.gravitychange(-player.gravityDirection);


            }

            previousDistance = distance;

        }

        if (radius >= maxRadius)

        {

            Destroy(gameObject);

        }

    }

    private void DrawCircle()

    {

        for (int i = 0; i < pointCount; i++)

        {

            float angle = 2f * Mathf.PI * i / pointCount;

            Vector3 point = new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0f
            );

            line.SetPosition(i, point);     //外
            coreLine.SetPosition(i, point);     //中


        }

    }

}
