using UnityEngine;

public class Gate : MonoBehaviour

{

    [Header("つながるゲート（AならB、BならAを入れる）")]

    public Gate destination;

    [Header("線の太さ")]

    public float width = 0.15f;

    [Header("プレイヤー")]

    public Player player;

    // true：通電中。触れると死亡

    // false：通電していない。通れる

    public bool GateOnOff;

    private LineRenderer line;

    public LineRenderer coreLine;

    // 前のフレームで線に触れていたか

    private bool wasTouching = false;

    public AudioClip gateSound;

    private AudioSource audioSource;

    public int pointCount = 12;

    public float shakeWidth = 0.04f;

    public float changeInterval = 0.06f;

    private float nextChangeTime = 0f;

    private float[] offsets;

    // 【追加】このゲートがスイッチから操作されているか

    private bool switchControlled = false;

    void Start()

    {

        line = GetComponent<LineRenderer>();

        audioSource = GetComponent<AudioSource>();

        line.useWorldSpace = true;

        coreLine.useWorldSpace = true;

        coreLine.sortingLayerID = line.sortingLayerID;

        coreLine.sortingOrder = line.sortingOrder + 1;

    }

    // 【追加】GateSwitchから呼ぶ

    // openがtrueなら開く、falseなら閉じる

    public void SetSwitchOpen(bool open)

    {

        switchControlled = true;

        GateOnOff = !open;

        // もう片方にも同じ状態を反映する

        if (destination != null)

        {

            destination.switchControlled = true;

            destination.GateOnOff = !open;

        }

    }

    void Update()

    {

        if (destination == null) return;

        Vector3 start = transform.position;

        Vector3 end = destination.transform.position;

        // ゲートAとBの真ん中

        Vector3 center = (start + end) / 2f;

        // 電気線の長さ・向き・角度

        float length = Vector3.Distance(start, end);

        Vector3 direction = end - start;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 電気線に沿った細長い四角で判定する

        Vector2 size = new Vector2(length, width);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, angle);

        bool touching = false;

        for (int i = 0; i < hits.Length; i++)

        {

            Collider2D hit = hits[i];

            if (player != null && hit.GetComponentInParent<Player>() == player)

            {

                touching = true;

                break;

            }

        }

        // 通常ゲートだけ、プレイヤーが通り抜けたら通電する

        // スイッチ式は、通り抜けても勝手に閉じない

        if (!switchControlled && wasTouching && !touching)

        {

            GateOnOff = true;

            if (audioSource != null && gateSound != null)

            {

                audioSource.PlayOneShot(gateSound);

            }

        }

        wasTouching = touching;

        if (GateOnOff && touching)

        {

            player.Deth();

        }

        // ここから下は電気線の見た目

        int count = Mathf.Max(2, pointCount);

        if (offsets == null || offsets.Length != count)

        {

            offsets = new float[count];

        }

        line.positionCount = count;

        coreLine.positionCount = count;

        // 電気線に対して直角の方向

        Vector3 side = new Vector3(-direction.y, direction.x, 0f).normalized;

        if (Time.time >= nextChangeTime)

        {

            int step = Mathf.FloorToInt(

                Time.time / Mathf.Max(0.01f, changeInterval)

            );

            for (int i = 1; i < count - 1; i++)

            {

                if ((i + step) % 2 == 0)

                {

                    offsets[i] = shakeWidth;

                }

                else

                {

                    offsets[i] = -shakeWidth;

                }

            }

            nextChangeTime = Time.time + changeInterval;

        }

        for (int i = 0; i < count; i++)

        {

            float t = (float)i / (count - 1);

            Vector3 point = start + direction * t;

            // 両端は固定して、途中の点だけずらす

            if (GateOnOff && i > 0 && i < count - 1)

            {

                point += side * offsets[i];

            }

            line.SetPosition(i, point);

            coreLine.SetPosition(i, point);

        }

        line.startWidth = width;

        line.endWidth = width;

        coreLine.startWidth = width / 3f;

        coreLine.endWidth = width / 3f;

        // 通常ゲート：通電前も灰色の線を表示

        // スイッチ式：開いている間は線を完全に消す

        line.enabled = !switchControlled || GateOnOff;

        coreLine.enabled = GateOnOff;

        if (GateOnOff)

        {

            line.startColor = Color.cyan;

            line.endColor = Color.cyan;

            coreLine.startColor = Color.white;

            coreLine.endColor = Color.white;

        }

        else

        {

            line.startColor = Color.gray;

            line.endColor = Color.gray;

        }

    }

}
