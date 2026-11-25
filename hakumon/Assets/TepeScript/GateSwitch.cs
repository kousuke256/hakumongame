using System.Collections.Generic;

using UnityEngine;

public class GateSwitch : MonoBehaviour

{

    public enum SwitchMode

    {

        押している間だけ,

        一度押したら開いたまま

    }

    [Header("開けたいゲート（AかBの片方だけ入れる）")]

    public Gate targetGate;

    [Header("スイッチの動作")]

    public SwitchMode mode = SwitchMode.押している間だけ;

    [Header("スイッチの見た目")]

    public SpriteRenderer switchRenderer;
    public Sprite offSprite;
    public Sprite onSprite;

    // 今スイッチに乗っているプレイヤーや箱の当たり判定

    private readonly HashSet<Collider2D> contacts = new HashSet<Collider2D>();

    // 一度押して、開いたままになったか

    private bool latched = false;

    public AudioClip gateOff;

    private AudioSource audioSource;

    void Awake()

    {
        audioSource = GetComponent<AudioSource>();

        switchRenderer.sprite = offSprite;

        // スイッチ式のゲートは閉じた状態で始める

        if (targetGate != null)

        {

            targetGate.SetSwitchOpen(false);

        }

    }

    void OnTriggerEnter2D(Collider2D other)

    {

        if (targetGate == null) return;

        bool isPlayer = other.GetComponentInParent<Player>() != null;

        bool isBox = other.GetComponentInParent<PushBox>() != null;

        // プレイヤーでも箱でもなければ押せない

        if (!isPlayer && !isBox) return;

        contacts.Add(other);

        contacts.Add(other);

        // すでに電気線が消えているなら、もう音は鳴らさない

        if (!targetGate.GateOnOff) return;

        if (mode == SwitchMode.一度押したら開いたまま)

        {

            latched = true;

        }

        targetGate.SetSwitchOpen(true);
        switchRenderer.sprite = onSprite;

        if (audioSource != null && gateOff != null)

        {

            audioSource.PlayOneShot(gateOff);

        }


    }

    void OnTriggerExit2D(Collider2D other)

    {

        // スイッチを押していなかったものは無視

        if (!contacts.Remove(other)) return;

        // 一度押したら開いたままのスイッチは閉じない

        if (latched) return;

        // 最後のプレイヤーか箱が離れたら閉じる

        if (contacts.Count == 0 && targetGate != null)

        {

            targetGate.SetSwitchOpen(false);

            switchRenderer.sprite = offSprite;

        }

    }

}
