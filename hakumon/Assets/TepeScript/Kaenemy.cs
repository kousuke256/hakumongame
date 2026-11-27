using UnityEngine;

public class Kaenemy : MonoBehaviour

{

    [Header("プレイヤー")]

    public Player player;

    [Header("発射する波のPrefab")]

    public GravityWave wavePrefab;

    [Header("最初の波を出すまでの秒数")]

    [Min(0f)] public float startDelay = 1f;

    private bool isFirstWave = true;


    [Header("何秒ごとに波を出すか")]

    [Min(0.1f)] public float interval = 3f;

    [Header("波が1秒で広がる距離")]

    [Min(0.1f)] public float waveSpeed = 8f;

    [Header("波が届く最大半径")]

    [Min(0.1f)] public float waveMaxRadius = 16f;

    private float timer;

    void Update()

    {

        if (player == null || wavePrefab == null)

            return;

        timer += Time.deltaTime;

        float waitTime = isFirstWave ? startDelay : interval;

        if (timer >= waitTime)
        {

            // 敵の位置に波を1個出す

            GravityWave wave = Instantiate(

                wavePrefab,

                transform.position,

                Quaternion.identity

            );

            // 今出した波に、この敵の設定を渡す

            wave.speed = waveSpeed;

            wave.maxRadius = waveMaxRadius;

            wave.SetPlayer(player);

            timer = 0f;
            isFirstWave = false;
        }

    }

    void OnCollisionEnter2D(Collision2D collision)

    {

        if (!collision.gameObject.CompareTag("Player"))

            return;

        // プレイヤーが敵を上から踏んだときはセーフ

        foreach (ContactPoint2D contact in collision.contacts)

        {

            if (contact.normal.y < -0.5f)

                return;

        }

        // それ以外の方向から触れたら、プレイヤー側の死亡処理を呼ぶ

        if (player != null)

            player.Deth();

    }

}
