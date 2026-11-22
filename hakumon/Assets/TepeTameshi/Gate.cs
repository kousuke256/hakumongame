
using UnityEngine;

public class Gate : MonoBehaviour

{

    [Header("つながるゲート(AならB,BならA入れる)")]

    public Gate destination;

    [Header("線の太さ")]

    public float width=0.15f;

    [Header("プレイヤー")]

    public Player player;

    public bool GateOnOff;

    private LineRenderer line;

    public LineRenderer coreLine;

    private bool wasTouching=false;         //線に触れていたか     

    public AudioClip gateSound;

    private AudioSource audioSource;

    public int pointCount = 12;
    public float shakeWidth = 0.04f;

    public float changeInterval = 0.06f;

    private float nextChangeTime = 0f;

    private float[] offsets;

    void Start()
    {
        line = GetComponent<LineRenderer>();

        line.useWorldSpace = true;

        coreLine.useWorldSpace = true;

        coreLine.sortingLayerID=line.sortingLayerID;
        coreLine.sortingOrder = line.sortingOrder + 1;

    }


    void Update()
    {
        

        if (destination == null) return;

        Vector3 start = transform.position;
        Vector3 end = destination.transform.position;
        Vector3 center = (start + end) / 2f;                //二つのベクトルの真ん中

        float length = Vector3.Distance(start, end);        //startからendまでの距離

        Vector3 direction = end - start;                    //向き
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;        //角度    

        Vector2 size = new Vector2(length, width);
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, angle);

        bool touching = false;

        for(int i=0;i<hits.Length; i++)
        {
            Collider2D hit = hits[i];
            if (player != null && hit.GetComponentInParent<Player>() == player)

            {

                touching = true;

            }

        }

        //通り抜けた瞬間
        if (wasTouching == true && touching == false)
        {
            GateOnOff = true;
            audioSource = GetComponent<AudioSource>();
            if (gateSound != null)
            {
                audioSource.PlayOneShot(gateSound);
            }
        }

        wasTouching = touching;

        if (GateOnOff == true && touching == true)
        {

            player.Deth();

        }



        int count = Mathf.Max(2, pointCount);

        if (offsets == null || offsets.Length != count)

        {

            offsets = new float[count];

        }


        line.positionCount = count;

        coreLine.positionCount = count;

        Vector3 side = new Vector3(
            -(end - start).y,
            (end - start).x,
            0f
            ).normalized;

        for (int i = 0; i < count; i++)

        {

            float t = (float)i / (count - 1);

            Vector3 point = start + (end - start) * t;

            if (GateOnOff && i > 0 && i < count - 1)

            {

                point += side * offsets[i];

            }


            line.SetPosition(i, point);

            coreLine.SetPosition(i, point);

        }

        if (Time.time >= nextChangeTime)

        {

            for (int i = 1; i < count - 1; i++)

            {

                int step = Mathf.FloorToInt(Time.time / Mathf.Max(0.01f, changeInterval)

);

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



        line.startWidth = width;
        line.endWidth = width;

        

        coreLine.startWidth = width / 3f;
        coreLine.endWidth = width / 3f;

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