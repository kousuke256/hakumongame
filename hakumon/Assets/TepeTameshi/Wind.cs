using UnityEngine;

[DefaultExecutionOrder(1000)]

public class Wind : MonoBehaviour

{

    [Header("プレイヤーのRigidbody2D")]

    public Rigidbody2D playerRb;

    [Header("風の範囲")]

    public Vector2 areaSize = new Vector2(5f, 4f);

    [Header("風向き：右0、上90、左180、下270")]

    [Range(0f, 360f)]

    public float windAngle = 90f;

    [Header("風で押す強さ")]

    [Min(0f)]

    public float windPower = 30f;

    [Header("風向きの速度が上がるほど風を弱める")]

    [Min(0.1f)]

    public float maxWindSpeed = 8f;

    [Header("出口側で風が弱まる範囲の割合")]

    [Range(0.05f, 1f)]

    public float fadeRatio = 0.3f;

    [Header("子オブジェクトのパーティクル")]

    public ParticleSystem windParticles;

    [Header("粒が流れる速さ")]

    [Min(0.1f)]

    public float particleSpeed = 4f;

    private Vector2 GetWindDirection()

    {

        float radians = windAngle * Mathf.Deg2Rad;

        return new Vector2(

            Mathf.Cos(radians),

            Mathf.Sin(radians)

        );

    }

    // 風エリアは画面に対して水平・垂直の長方形

    private Vector2 GetWorldSize()

    {

        return new Vector2(

            areaSize.x * Mathf.Abs(transform.lossyScale.x),

            areaSize.y * Mathf.Abs(transform.lossyScale.y)

        );

    }

    private void FixedUpdate()

    {

        if (playerRb == null || !playerRb.simulated)

            return;

        Vector2 size = GetWorldSize();

        if (size.x <= 0f || size.y <= 0f)

            return;

        Vector2 center = transform.position;

        Vector2 offset = playerRb.position - center;

        Vector2 halfSize = size * 0.5f;

        // 範囲外には風を加えない

        if (Mathf.Abs(offset.x) > halfSize.x ||

            Mathf.Abs(offset.y) > halfSize.y)

        {

            return;

        }

        Vector2 direction = GetWindDirection();

        // 今の位置から、風向きに進んだ先の出口までの距離

        float distanceX = float.PositiveInfinity;

        float distanceY = float.PositiveInfinity;

        if (Mathf.Abs(direction.x) > 0.0001f)

        {

            distanceX =

                (halfSize.x - Mathf.Sign(direction.x) * offset.x)

                / Mathf.Abs(direction.x);

        }

        if (Mathf.Abs(direction.y) > 0.0001f)

        {

            distanceY =

                (halfSize.y - Mathf.Sign(direction.y) * offset.y)

                / Mathf.Abs(direction.y);

        }

        float distanceToExit = Mathf.Min(distanceX, distanceY);

        // エリア中心を通る場合の、風向きに沿った通過距離

        float fullDistance = GetTravelDistance(size, direction);

        float fadeDistance = Mathf.Max(

            0.01f,

            fullDistance * fadeRatio

        );

        // 出口に近いほど風が弱くなる

        float positionMultiplier = Mathf.Clamp01(

            distanceToExit / fadeDistance

        );

        // 風向きの現在速度

        float speedAlongWind = Vector2.Dot(

            playerRb.linearVelocity,

            direction

        );

        // 風と同じ方向に速く進んでいるほど押す力を弱める

        // 逆方向に落下しているときは強めに受け止める

        float speedMultiplier = Mathf.Clamp(

            1f - speedAlongWind / Mathf.Max(0.1f, maxWindSpeed),

            0f,

            2f

        );

        float acceleration =

            windPower * positionMultiplier * speedMultiplier;

        // 風向きに押すだけで、逆向きには引き戻さない

        playerRb.AddForce(

            direction * acceleration * playerRb.mass,

            ForceMode2D.Force

        );

    }

    private float GetTravelDistance(

        Vector2 size,

        Vector2 direction

    )

    {

        float travelX = float.PositiveInfinity;

        float travelY = float.PositiveInfinity;

        if (Mathf.Abs(direction.x) > 0.0001f)

            travelX = size.x / Mathf.Abs(direction.x);

        if (Mathf.Abs(direction.y) > 0.0001f)

            travelY = size.y / Mathf.Abs(direction.y);

        return Mathf.Min(travelX, travelY);

    }

    private void Start()

    {

        UpdateParticles();

    }

    private void LateUpdate()

    {

        UpdateParticles();

    }

    private void UpdateParticles()

    {

        if (windParticles == null)

            return;

        // 風本体に付けたParticle Systemは使用しない

        if (windParticles.transform == transform)

            return;

        Vector2 direction = GetWindDirection();

        Vector2 size = GetWorldSize();

        Vector3 spawnPosition = transform.position;

        Vector3 spawnSize;

        // 風向きと反対側の辺から粒を出す

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))

        {

            spawnPosition.x -=

                Mathf.Sign(direction.x) * size.x * 0.5f;

            spawnSize = new Vector3(0.01f, size.y, 0.01f);

        }

        else

        {

            spawnPosition.y -=

                Mathf.Sign(direction.y) * size.y * 0.5f;

            spawnSize = new Vector3(size.x, 0.01f, 0.01f);

        }

        windParticles.transform.position = spawnPosition;

        windParticles.transform.rotation = Quaternion.identity;

        var main = windParticles.main;

        main.startSpeed = 0f;

        main.gravityModifier = 0f;

        main.scalingMode = ParticleSystemScalingMode.Shape;

        // 上下左右の風では、反対の辺付近まで流れる寿命

        main.startLifetime = Mathf.Max(

            0.01f,

            GetTravelDistance(size, direction)

            / Mathf.Max(0.1f, particleSpeed)

        );

        var velocity = windParticles.velocityOverLifetime;

        velocity.enabled = true;

        velocity.space = ParticleSystemSimulationSpace.World;

        velocity.x = direction.x * particleSpeed;

        velocity.y = direction.y * particleSpeed;

        velocity.z = 0f;

        Vector3 scale = windParticles.transform.lossyScale;

        var shape = windParticles.shape;

        shape.enabled = true;

        shape.shapeType = ParticleSystemShapeType.Box;

        shape.position = Vector3.zero;

        shape.rotation = Vector3.zero;

        shape.scale = new Vector3(

            spawnSize.x / Mathf.Max(Mathf.Abs(scale.x), 0.0001f),

            spawnSize.y / Mathf.Max(Mathf.Abs(scale.y), 0.0001f),

            spawnSize.z / Mathf.Max(Mathf.Abs(scale.z), 0.0001f)

        );

    }

    private void OnValidate()

    {

        areaSize.x = Mathf.Max(0.1f, areaSize.x);

        areaSize.y = Mathf.Max(0.1f, areaSize.y);

        windPower = Mathf.Max(0f, windPower);

        maxWindSpeed = Mathf.Max(0.1f, maxWindSpeed);

        particleSpeed = Mathf.Max(0.1f, particleSpeed);

        fadeRatio = Mathf.Clamp(fadeRatio, 0.05f, 1f);

    }

    private void OnDrawGizmos()

    {

        Vector2 size = GetWorldSize();

        Vector3 boxSize = new Vector3(size.x, size.y, 0.05f);

        Vector3 center = transform.position;

        Gizmos.color = new Color(0f, 1f, 1f, 0.1f);

        Gizmos.DrawCube(center, boxSize);

        Gizmos.color = Color.cyan;

        Gizmos.DrawWireCube(center, boxSize);

        Vector3 direction = GetWindDirection();

        Vector3 tip = center + direction * 1.5f;

        Vector3 side = new Vector3(-direction.y, direction.x, 0f);

        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(center, tip);

        Gizmos.DrawLine(

            tip,

            tip - direction * 0.35f + side * 0.2f

        );

        Gizmos.DrawLine(

            tip,

            tip - direction * 0.35f - side * 0.2f

        );

    }

}
