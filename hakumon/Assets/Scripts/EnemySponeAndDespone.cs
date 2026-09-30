using UnityEngine;

public class EnemySponeAndDespone : MonoBehaviour
{
    [SerializeField] private float spawnMargin = 2f;   // 画面の少し外でスポーン
    [SerializeField] private float despawnMargin = 10f; // もっと外でデスポーン

    private bool isSpawned = false;

    void Update()
    {
        Camera cam = Camera.main;
        float camX = cam.transform.position.x;

        // カメラの半分の幅（OrthographicSize と Aspect から計算）
        float halfWidth = cam.orthographicSize * cam.aspect;

        // スポーンライン（画面の少し外）
        float spawnLeft = camX - halfWidth - spawnMargin;
        float spawnRight = camX + halfWidth + spawnMargin;

        // デスポーンライン（画面のもっと外）
        float despawnLeft = camX - halfWidth - despawnMargin;
        float despawnRight = camX + halfWidth + despawnMargin;

        float mobX = transform.position.x;

        // スポーン判定
        if (!isSpawned && mobX > spawnLeft && mobX < spawnRight)
        {
            Spawn();
        }

        // デスポーン判定
        if (isSpawned && (mobX < despawnLeft || mobX > despawnRight))
        {
            Despawn();
        }
    }

    private void Spawn()
    {
        gameObject.SetActive(true);
        isSpawned = true;
    }

    private void Despawn()
    {
        gameObject.SetActive(false);
        isSpawned = false;
    }
}

