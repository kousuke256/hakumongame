using System.Collections;

using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class GravitySniper : MonoBehaviour

{

    [Header("狙うプレイヤー")]

    public Player player;

    [Header("ビームのPrefab")]

    public GravitySniperBullet bulletPrefab;

    [Header("銃口（空欄なら砲台の中心）")]

    public Transform firePoint;

    [Header("敵が重力反転に反応する距離")]

    [Min(0.1f)] public float detectRange = 16f;

    [Header("予告線を出してから撃つまでの秒数")]

    [Min(0f)] public float warningTime = 0.15f;

    [Header("ビームの先端が進む速さ")]

    [Min(0.1f)] public float bulletSpeed = 20f;

    [Header("予告線の見た目")]

    public Color warningColor = Color.red;

    [Min(0.01f)] public float lineWidth = 0.05f;

    private LineRenderer aimLine;

    private Vector2 previousGravity;

    private bool isAiming;

    void Awake()

    {

        aimLine = GetComponent<LineRenderer>();

        aimLine.useWorldSpace = true;

        aimLine.loop = false;

        aimLine.positionCount = 2;

        aimLine.startWidth = aimLine.endWidth = lineWidth;

        aimLine.startColor = aimLine.endColor = warningColor;

        aimLine.enabled = false;

    }

    void OnEnable()

    {

        // 開始時の重力を覚える。開始しただけでは撃たない

        if (player != null)

            previousGravity = player.gravityDirection;

    }

    void LateUpdate()

    {

        if (player == null || bulletPrefab == null) return;

        Vector2 currentGravity = player.gravityDirection;

        // 前のフレームと重力が同じなら何もしない

        if (currentGravity == previousGravity) return;

        previousGravity = currentGravity;

        if (isAiming) return;

        Vector2 start = firePoint != null

            ? (Vector2)firePoint.position

            : (Vector2)transform.position;

        Vector2 target = player.transform.position;

        float distance = Vector2.Distance(start, target);

        if (distance > detectRange || distance < 0.001f) return;

        StartCoroutine(Fire(start, target));

    }

    private IEnumerator Fire(Vector2 start, Vector2 target)

    {

        isAiming = true;

        // 反転した瞬間の位置に照準を固定する

        aimLine.SetPosition(0, start);

        aimLine.SetPosition(1, target);

        aimLine.enabled = true;

        if (warningTime > 0f)

            yield return new WaitForSeconds(warningTime);

        aimLine.enabled = false;

        Vector2 direction = (target - start).normalized;

        GravitySniperBullet beam =

            Instantiate(bulletPrefab, start, Quaternion.identity);

        beam.Shoot(direction, bulletSpeed, player);

        isAiming = false;

    }

    void OnDisable()

    {

        StopAllCoroutines();

        isAiming = false;

        if (aimLine != null)

            aimLine.enabled = false;

    }

}
