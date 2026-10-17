using UnityEngine;

public class OnOffver2 : MonoBehaviour
{
    public bool OnOff;                 // ON/OFF の初期状態
    public Sprite OnSprite;            // ON の見た目
    public Sprite OffSprite;           // OFF の見た目

    private Player PlayerScript;       // 自動取得する
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    private float oldGravityDirectionY;

    

    void Start()
    {
         GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null)
        {
            PlayerScript = obj.GetComponent<Player>();
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (PlayerScript == null)
        {
            Debug.LogError("OnOffBlock: Player が見つかりません！タグ 'Player' を設定してください。", this);
            enabled = false;
            return;
        }

        if (spriteRenderer == null)
        {
            Debug.LogError("OnOffBlock: SpriteRenderer がありません！", this);
            enabled = false;
            return;
        }

        if (boxCollider == null)
        {
            Debug.LogError("OnOffBlock: BoxCollider2D がありません！", this);
            enabled = false;
            return;
        }

        oldGravityDirectionY = PlayerScript.gravityDirection.y;

        UpdateBlock();
    }

    void Update()
    {
        float currentGravityY = PlayerScript.gravityDirection.y;

        if (Mathf.Approximately(oldGravityDirectionY, currentGravityY))
            return;

        oldGravityDirectionY = currentGravityY;

        UpdateBlock();
    }

    void UpdateBlock()
    {
        bool reverseGravity = PlayerScript.gravityDirection.y > 0f;

        bool active = reverseGravity ? !OnOff : OnOff;

        if (active)
        {
            spriteRenderer.sprite = OnSprite;

            boxCollider.enabled = true;
            boxCollider.isTrigger = false;

            gameObject.layer = LayerMask.NameToLayer("Ground");
            gameObject.tag = "Ground";
        }
        else
        {
            spriteRenderer.sprite = OffSprite;

            boxCollider.enabled = true;
            boxCollider.isTrigger = true;

            gameObject.layer = LayerMask.NameToLayer("Off");
            gameObject.tag = "Untagged";
        }
    }
}

