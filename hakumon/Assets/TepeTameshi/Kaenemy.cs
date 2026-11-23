using UnityEngine;
using UnityEngine.SceneManagement;

public class Kaenemy : MonoBehaviour

{

    public Player player;
    private bool canMove;
    private float y = 1;
    private bool isDead = false;
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


    void OnCollisionEnter2D(Collision2D collision)

    {

        if (!collision.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint2D contact in collision.contacts)

        {

            // 踏まれたときはプレイヤーを倒さない

            if (y == 1 && contact.normal.y < -0.5f) return;

            if (y != 1 && contact.normal.y > 0.5f) return;

        }

        player.Deth();

    }

}




