using System.Collections.Generic;

using UnityEngine;

using UnityEngine.SceneManagement;

public class WarpPortal : MonoBehaviour

{

    [Header("つながる相手のワープ(このワープではない方)")]

    public WarpPortal destination;

    [Header("このワープから出る位置(このワープの子オブジェクト)")]

    public Transform exitPoint;

    [Header("連続ワープを防ぐ時間")]

    [Min(0.1f)]

    public float cooldown = 0.5f;

    // プレイヤーごとの待ち時間を全ワープで共有

    private static readonly Dictionary<Rigidbody2D, float> nextWarpTime

        = new Dictionary<Rigidbody2D, float>();

    [RuntimeInitializeOnLoadMethod(

        RuntimeInitializeLoadType.SubsystemRegistration)]

    private static void Initialize()

    {

        nextWarpTime.Clear();

        SceneManager.sceneUnloaded -= OnSceneUnloaded;

        SceneManager.sceneUnloaded += OnSceneUnloaded;

    }

    private static void OnSceneUnloaded(Scene scene)

    {

        // シーンを離れたら古いプレイヤーの情報を消す

        nextWarpTime.Clear();

    }

    private void OnTriggerEnter2D(Collider2D other)

    {

        Rigidbody2D rb = other.attachedRigidbody;

        if (rb == null) return;

        //Player player = rb.GetComponent<Player>();

        //if (player == null) return;

        // 移動先が未設定、または自分自身なら何もしない

        if (destination == null || destination == this)

            return;

        if (!destination.isActiveAndEnabled)

            return;

        if (destination.exitPoint == null)

        {

            Debug.LogWarning(

                "移動先のワープにExit Pointを設定してください！",

                destination

            );

            return;

        }

        // 移動した直後なら再びワープしない

        if (nextWarpTime.TryGetValue(rb, out float allowedTime)
&& Time.time < allowedTime)

        {

            return;

        }

        nextWarpTime[rb] = Time.time

            + Mathf.Max(0.1f, Mathf.Max(

                cooldown,

                destination.cooldown

            ));

        // 入ったときの速度を保存

        Vector2 savedVelocity = rb.linearVelocity;

        // 移動先の出口へ瞬間移動

        Vector3 targetPosition = destination.exitPoint.position;

        // 奥行きは現在のままにする

        targetPosition.z = rb.transform.position.z;

        rb.transform.position = targetPosition;

        rb.position = new Vector2(

            targetPosition.x,

            targetPosition.y

        );

        // 横移動・落下の勢いを維持する

        rb.linearVelocity = savedVelocity;

        // 重力の向きは変更しない

    }

}
