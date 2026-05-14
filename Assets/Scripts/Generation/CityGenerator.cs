using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CityGenerator : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap roadTilemap;
    [SerializeField] private Tilemap groundTilemap;

    [Header("Tiles")]
    [SerializeField] private TileBase roadTile;
    [SerializeField] private TileBase sidewalkTile;

    [Header("Prefabs")]
    [SerializeField] private GameObject packagePrefab;
    [SerializeField] private GameObject dropzonePrefab;

    [Header("References")]
    [SerializeField] private PackageManager packageManager;

    [Header("Generation Settings")]
    [SerializeField] private int gridWidth  = 5;
    [SerializeField] private int gridHeight = 5;
    [SerializeField] private int blockSize  = 6;
    [SerializeField] private int roadWidth  = 3;
    [SerializeField] private float extraEdgeProbability = 0.30f;

    // edgeH[gx, gy] = road corridor connecting (gx,gy) and (gx+1,gy)
    private bool[,] edgeH;
    // edgeV[gx, gy] = road corridor connecting (gx,gy) and (gx,gy+1)
    private bool[,] edgeV;
    private List<Vector2Int> deadEnds;
    private System.Random rng;

    void Awake()
    {
        rng = new System.Random(LevelSeedConfig.Seed);
        GenerateGraph();
        PaintTilemaps();
        SpawnPickups();
    }

    // -------------------------------------------------------------------------
    // Graph generation
    // -------------------------------------------------------------------------

    void GenerateGraph()
    {
        edgeH = new bool[gridWidth - 1, gridHeight];
        edgeV = new bool[gridWidth, gridHeight - 1];

        bool[,] visited = new bool[gridWidth, gridHeight];
        var stack = new Stack<Vector2Int>();

        int startX = rng.Next(gridWidth);
        int startY = rng.Next(gridHeight);
        var start = new Vector2Int(startX, startY);
        visited[start.x, start.y] = true;
        stack.Push(start);

        // DFS spanning tree
        while (stack.Count > 0)
        {
            var current = stack.Peek();
            var unvisited = GetUnvisitedNeighbors(current, visited);

            if (unvisited.Count == 0)
            {
                stack.Pop();
                continue;
            }

            var next = unvisited[rng.Next(unvisited.Count)];
            MarkEdge(current, next);
            visited[next.x, next.y] = true;
            stack.Push(next);
        }

        // Add ~30% extra edges for loops
        for (int gx = 0; gx < gridWidth - 1; gx++)
            for (int gy = 0; gy < gridHeight; gy++)
                if (!edgeH[gx, gy] && rng.NextDouble() < extraEdgeProbability)
                    edgeH[gx, gy] = true;

        for (int gx = 0; gx < gridWidth; gx++)
            for (int gy = 0; gy < gridHeight - 1; gy++)
                if (!edgeV[gx, gy] && rng.NextDouble() < extraEdgeProbability)
                    edgeV[gx, gy] = true;

        // Find dead ends
        deadEnds = new List<Vector2Int>();
        for (int gx = 0; gx < gridWidth; gx++)
            for (int gy = 0; gy < gridHeight; gy++)
                if (GetDegree(new Vector2Int(gx, gy)) == 1)
                    deadEnds.Add(new Vector2Int(gx, gy));
    }

    void MarkEdge(Vector2Int a, Vector2Int b)
    {
        if (a.x != b.x)
            edgeH[Mathf.Min(a.x, b.x), a.y] = true;
        else
            edgeV[a.x, Mathf.Min(a.y, b.y)] = true;
    }

    int GetDegree(Vector2Int node)
    {
        int count = 0;
        if (node.x > 0              && edgeH[node.x - 1, node.y]) count++;
        if (node.x < gridWidth - 1  && edgeH[node.x,     node.y]) count++;
        if (node.y > 0              && edgeV[node.x, node.y - 1]) count++;
        if (node.y < gridHeight - 1 && edgeV[node.x, node.y    ]) count++;
        return count;
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int node, bool[,] visited)
    {
        var result = new List<Vector2Int>(4);
        TryAdd(new Vector2Int(node.x - 1, node.y), visited, result);
        TryAdd(new Vector2Int(node.x + 1, node.y), visited, result);
        TryAdd(new Vector2Int(node.x, node.y - 1), visited, result);
        TryAdd(new Vector2Int(node.x, node.y + 1), visited, result);
        return result;
    }

    void TryAdd(Vector2Int n, bool[,] visited, List<Vector2Int> list)
    {
        if (n.x >= 0 && n.x < gridWidth && n.y >= 0 && n.y < gridHeight && !visited[n.x, n.y])
            list.Add(n);
    }

    // -------------------------------------------------------------------------
    // Tilemap painting
    // -------------------------------------------------------------------------

    void PaintTilemaps()
    {
        roadTilemap.ClearAllTiles();
        groundTilemap.ClearAllTiles();

        int stride = blockSize + roadWidth;
        var roadPos = new HashSet<Vector2Int>();

        // Intersection squares at every grid node
        for (int gx = 0; gx < gridWidth; gx++)
        {
            for (int gy = 0; gy < gridHeight; gy++)
            {
                int ox = gx * stride;
                int oy = gy * stride;
                for (int dx = 0; dx < roadWidth; dx++)
                    for (int dy = 0; dy < roadWidth; dy++)
                        roadPos.Add(new Vector2Int(ox + dx, oy + dy));
            }
        }

        // Horizontal corridors
        for (int gx = 0; gx < gridWidth - 1; gx++)
        {
            for (int gy = 0; gy < gridHeight; gy++)
            {
                if (!edgeH[gx, gy]) continue;
                int startX = gx * stride + roadWidth;
                int endX   = (gx + 1) * stride;        // exclusive
                int originY = gy * stride;
                for (int x = startX; x < endX; x++)
                    for (int dy = 0; dy < roadWidth; dy++)
                        roadPos.Add(new Vector2Int(x, originY + dy));
            }
        }

        // Vertical corridors
        for (int gx = 0; gx < gridWidth; gx++)
        {
            for (int gy = 0; gy < gridHeight - 1; gy++)
            {
                if (!edgeV[gx, gy]) continue;
                int startY = gy * stride + roadWidth;
                int endY   = (gy + 1) * stride;        // exclusive
                int originX = gx * stride;
                for (int y = startY; y < endY; y++)
                    for (int dx = 0; dx < roadWidth; dx++)
                        roadPos.Add(new Vector2Int(originX + dx, y));
            }
        }

        // Paint road tiles
        foreach (var pos in roadPos)
            roadTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), roadTile);

        // Paint sidewalk border (8-directional neighbors not in road)
        var sidewalkPos = new HashSet<Vector2Int>();
        int[] dd = { -1, 0, 1 };
        foreach (var pos in roadPos)
        {
            foreach (int dx in dd)
            {
                foreach (int dy in dd)
                {
                    if (dx == 0 && dy == 0) continue;
                    var n = new Vector2Int(pos.x + dx, pos.y + dy);
                    if (!roadPos.Contains(n))
                        sidewalkPos.Add(n);
                }
            }
        }

        foreach (var pos in sidewalkPos)
            groundTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), sidewalkTile);
    }

    // -------------------------------------------------------------------------
    // Package / dropzone spawning
    // -------------------------------------------------------------------------

    void SpawnPickups()
    {
        int stride = blockSize + roadWidth;

        // Shuffle dead ends for variety
        Shuffle(deadEnds);

        var packageGOs  = new List<GameObject>();
        var dropzoneGOs = new List<GameObject>();

        // Packages at dead-end intersections
        foreach (var node in deadEnds)
        {
            var worldPos = NodeToWorld(node, stride);
            packageGOs.Add(Instantiate(packagePrefab, worldPos, Quaternion.identity));
        }

        // Dropzones at non-dead-end nodes
        var nonDeadEnds = new List<Vector2Int>();
        for (int gx = 0; gx < gridWidth; gx++)
            for (int gy = 0; gy < gridHeight; gy++)
                if (!deadEnds.Contains(new Vector2Int(gx, gy)))
                    nonDeadEnds.Add(new Vector2Int(gx, gy));

        Shuffle(nonDeadEnds);

        int dropzoneCount = Mathf.Min(packageGOs.Count, nonDeadEnds.Count);
        for (int i = 0; i < dropzoneCount; i++)
        {
            var worldPos = NodeToWorld(nonDeadEnds[i], stride);
            dropzoneGOs.Add(Instantiate(dropzonePrefab, worldPos, Quaternion.identity));
        }

        // Trim packages to match dropzone count if we ran out of non-dead-end nodes
        while (packageGOs.Count > dropzoneGOs.Count)
        {
            Destroy(packageGOs[packageGOs.Count - 1]);
            packageGOs.RemoveAt(packageGOs.Count - 1);
        }

        packageManager.InitPackages(packageGOs.ToArray(), dropzoneGOs.ToArray());
    }

    Vector3 NodeToWorld(Vector2Int node, int stride)
    {
        // Center of the intersection square for this node
        int tileX = node.x * stride + roadWidth / 2;
        int tileY = node.y * stride + roadWidth / 2;
        return roadTilemap.GetCellCenterWorld(new Vector3Int(tileX, tileY, 0));
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
