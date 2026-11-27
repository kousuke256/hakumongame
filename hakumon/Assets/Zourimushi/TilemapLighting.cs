using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class TilemapLighting : MonoBehaviour
{
    private int height;

    [Header("地面のTilemap")]
    [SerializeField] private Tilemap groundTilemap;

    [Header("明るさの設定")]
    [SerializeField] private int fadeDistance = 6;
    [SerializeField] private Color surfaceColor = Color.white;
    [SerializeField] private Color deepColor = new Color(0.2f, 0.2f, 0.25f, 1f);

    // 上下左右の隣接セル
    private readonly Vector3Int[] directions =
    {
        Vector3Int.up,
        Vector3Int.down,
        Vector3Int.left,
        Vector3Int.right
    };

    private IEnumerator Start()
    {
        // カメラの表示範囲の高さ
        height = Mathf.FloorToInt(Camera.main.orthographicSize);
        yield return null;

        UpdateLighting();
    }

    public void UpdateLighting()
    {
        if (groundTilemap == null) return;

        BoundsInt bounds = groundTilemap.cellBounds;

        // 地面が存在するセルを記録
        HashSet<Vector3Int> solidCells = new HashSet<Vector3Int>();

        // 表面からの距離を記録
        Dictionary<Vector3Int, int> distances = new Dictionary<Vector3Int, int>();

        Queue<Vector3Int> queue = new Queue<Vector3Int>();

        // 地面の存在するセルを取得
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            if (groundTilemap.HasTile(pos))
            {
                solidCells.Add(pos);
            }
        }

        // 表面のセルを探す
        foreach (Vector3Int pos in solidCells)
        {
            foreach (Vector3Int dir in directions)
            {
                Vector3Int neighbor = pos + dir;

                if(Mathf.Abs(neighbor.y) > height) continue;

                // 隣が空気なら表面
                if (!solidCells.Contains(neighbor))
                {
                    distances[pos] = 0;
                    queue.Enqueue(pos);
                    break;
                }
            }
        }

        // 表面から内側への距離を計算
        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            foreach (Vector3Int dir in directions)
            {
                Vector3Int next = current + dir;

                if (!solidCells.Contains(next)) continue;

                if (distances.ContainsKey(next)) continue;

                distances[next] = distances[current] + 1;
                queue.Enqueue(next);
            }
        }

        // 距離に応じて色を変更
                    int maxDistance = 0;
        foreach (Vector3Int pos in solidCells)
        {
            int distance = distances[pos];

                        maxDistance = Mathf.Max(maxDistance, distance);

            float t = Mathf.Clamp01((float)distance / fadeDistance);

            Color color = Color.Lerp(surfaceColor, deepColor, t);

            // タイルの色変更を許可する
            groundTilemap.SetTileFlags(pos, TileFlags.None);

            groundTilemap.SetColor(pos, color);
        }
    }
    
}