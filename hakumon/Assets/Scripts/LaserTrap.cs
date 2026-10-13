using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LaserTrap : MonoBehaviour
{
    public float offTime = 3f;
    public float onTime = 1f;
    [Range(0.1f, 0.5f)]
    public float bodyHeight = 0.27f;
    [Range(0.01f, 0.3f)]
    public float beamWidth = 0.08f;
    private GameObject beamObject;
    private BoxCollider2D beamCollider;
    private bool firing;
    private bool hit;
    IEnumerator Start()
    {
        SpriteRenderer original = GetComponent<SpriteRenderer>();
        if (original == null || original.sprite == null)
        {
            Debug.LogError("レーザー画像のSprite Rendererが必要です");
            yield break;
        }
        Sprite source = original.sprite;
        Rect rect = source.rect;
        float cut = Mathf.Round(rect.height * bodyHeight);
        float ppu = source.pixelsPerUnit;
        // 下側を本体、上側をビームとして作る
        CreatePart("Body", original, source, 0, cut);
        beamObject = CreatePart(
            "Beam", original, source, cut, rect.height - cut
        );
        original.enabled = false;
        // 当たり判定は細いビーム部分だけ
        beamCollider = gameObject.AddComponent<BoxCollider2D>();
        beamCollider.isTrigger = true;
        beamCollider.size = new Vector2(
            rect.width * beamWidth / ppu,
            (rect.height - cut) / ppu
        );
        beamCollider.offset = new Vector2(
            (rect.width * 0.5f - source.pivot.x) / ppu,
            (cut + (rect.height - cut) * 0.5f
                - source.pivot.y) / ppu
        );
        // プレイヤーとの接触を検出する
        if (GetComponent<Rigidbody2D>() == null)
        {
            Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        while (true)
        {
            SetLaser(false);
            yield return new WaitForSeconds(offTime);
            SetLaser(true);
            yield return new WaitForSeconds(onTime);
        }
    }
    GameObject CreatePart(
        string partName, SpriteRenderer original,
        Sprite source, float bottom, float height)
    {
        Rect rect = source.rect;
        Sprite partSprite = Sprite.Create(
            source.texture,
            new Rect(rect.x, rect.y + bottom, rect.width, height),
            new Vector2(0.5f, 0.5f),
            source.pixelsPerUnit
        );
        GameObject part = new GameObject(partName);
        part.transform.SetParent(transform, false);
        part.transform.localPosition = new Vector3(
            (rect.width * 0.5f - source.pivot.x)
                / source.pixelsPerUnit,
            (bottom + height * 0.5f - source.pivot.y)
                / source.pixelsPerUnit,
            0
        );
        SpriteRenderer sr = part.AddComponent<SpriteRenderer>();
        sr.sprite = partSprite;
        sr.sharedMaterial = original.sharedMaterial;
        sr.color = original.color;
        sr.sortingLayerID = original.sortingLayerID;
        sr.sortingOrder = original.sortingOrder;
        return part;
    }
    void SetLaser(bool active)
    {
        firing = active;
        beamObject.SetActive(active);
        beamCollider.enabled = active;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!firing || hit || !other.CompareTag("Player"))
            return;
        hit = true;
        // レーザーに触れたら現在のシーンをやり直す
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}