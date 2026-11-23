using UnityEngine;

public class Kaenemy : MonoBehaviour

{

    public Player player;

    public GravityWave wavePrefab;

    [Header("何秒ごとに波を出すか")]

    [Min(0.1f)] public float interval = 3f;

    private float timer;

    void Update()

    {

        if (player == null || wavePrefab == null) return;

        timer += Time.deltaTime;

        if (timer >= interval)

        {

            // 敵の現在位置に波を1個出す

            GravityWave wave = Instantiate(

                wavePrefab, transform.position, Quaternion.identity

            );

            wave.SetPlayer(player);

            timer = 0f;

        }

    }

}
