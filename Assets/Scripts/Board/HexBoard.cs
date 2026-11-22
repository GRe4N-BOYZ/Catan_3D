using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class HexBoard : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
        [Header("Prefabs")] // 👈 これを書くと、Inspector上で太字の見出しがつくで！
    public GameObject hexPrefab;
    public GameObject vertexPrefab;
    public GameObject EdgePrefab;
    public GameObject settlementPrefab;
    public GameObject harborPrefab;


    Dictionary<Vector3, Vertex> vertices =
    new Dictionary<Vector3, Vertex>();

    List<Vertex> allVertices = new List<Vertex>();
    List<Edge> allEdges = new List<Edge>();
    HashSet<string> edgeKeys = new HashSet<string>();

    float xOffset = Mathf.Sqrt(3);
    float zOffset = 1.5f;
    void Start()
    {

        
        // 1. アルゴリズム専用クラスをインスタンス化し、公式配置データを取得する
        BoardGenerator generator = new BoardGenerator();
        BoardData balancedData = generator.GenerateBoard(GameSettings.SelectedDifficulty);

        // 2. そのデータを渡して、シーン上にオブジェクトを配置する
        CreateHexBoard(balancedData);
        CreateHarbors();

        // 3. 【既存のカメラ連携】盤面が完成した直後にカメラを真ん中に移動させる
        CameraController cameraController = FindFirstObjectByType<CameraController>();
        if (cameraController != null)
        {
            cameraController.SetToBoardCenter();
        }
    }

     // 【引数をBoardDataに変更】
    void CreateHexBoard(BoardData boardData)
    {
        int[] rows = { 3, 4, 5, 4, 3 };
        int tileIndex = 0;

        for (int row = 0; row < rows.Length; row++)
        {
            int count = rows[row];
            float startX = -(count - 1) * xOffset / 2f;

            for (int col = 0; col < count; col++)
            {
                float xPos = startX + col * xOffset;
                float zPos = row * zOffset;
                Vector3 pos = new Vector3(xPos, 0, zPos);

                GameObject tileObject = Instantiate(hexPrefab, pos, Quaternion.identity);
                HexTile tile = tileObject.GetComponent<HexTile>();

                CreateVertices(pos, tile);

                TileData data = boardData.tiles[tileIndex++];
                tile.resourceType = data.resourceType;

                if (data.resourceType == ResourceType.Desert)
                {
                    tile.hasRobber = true;
                    // 【追加】砂漠に最初の盗賊コマを出現させる
                    RobberManager.Instance?.InitializeRobber(tile);
                }
                else
                {
                    tile.numberToken = data.numberToken;
                }
            }
        }
        CreateEdges();
    }

    void CreateVertices(Vector3 center, HexTile tile)
    {
        for(int i = 0; i < 6; i++)
        {
            float angle = Mathf.Deg2Rad * (60 * i + 90);
            float vertexRadius = 1.0f;
            
            Vector3 offset = new Vector3
            (
                Mathf.Cos(angle) * vertexRadius,
                0.2f,
                Mathf.Sin(angle) * vertexRadius
            );

            Vector3 vertexPos = center + offset;

            Vector3 keyPos = new Vector3(
                Mathf.Round(vertexPos.x * 100f) / 100f,
                0,
                Mathf.Round(vertexPos.z * 100f) / 100f
            );

            Vertex vertex;

            if(vertices.ContainsKey(keyPos))
            {
                vertex = vertices[keyPos];
            }
            else
            {
                GameObject obj = Instantiate
                (
                    vertexPrefab,
                    vertexPos,
                    Quaternion.identity
                );

                vertex = obj.GetComponent<Vertex>();

                vertices.Add
                (
                    keyPos,
                    vertex
                );

                allVertices.Add(vertex);
            }

            tile.adjacentVertices.Add(vertex);

            vertex.adjacentTiles.Add(tile);
        }
    }

    void CreateEdges()
    {
        for(int i = 0; i < allVertices.Count; i++)
        {
            for(int j = i + 1; j < allVertices.Count; j++)
            {
                if(i == j)
                {
                    continue;
                }
                float distance = Vector3.Distance
                (
                    allVertices[i].transform.position,
                    allVertices[j].transform.position
                );
                //Debug.Log(distance);

                if(distance > 0.9f && distance < 1.1f)
                {
                    CreateEdge
                    (
                        allVertices[i],
                        allVertices[j]
                    );
                }
            }
        }
    }

    List<ResourceType> CreateResources()
    {
        List<ResourceType> resources =
            new List<ResourceType>();
        
        resources.Add(ResourceType.Desert);

        for(int i = 0; i < 4; i++)
            resources.Add(ResourceType.Wood);

        for(int i = 0; i < 4; i++)
            resources.Add(ResourceType.Sheep);
        
        for(int i = 0; i < 4; i++)
            resources.Add(ResourceType.Wheat);

        for(int i = 0; i < 3; i++)
            resources.Add(ResourceType.Brick);
        
        for(int i = 0; i < 3; i++)
            resources.Add(ResourceType.Ore);

        return resources;
    }

    List<int> CreateNumbers()
    {
        return new List<int>
        {
            2,
            3,3,
            4,4,
            5,5,
            6,6,
            8,8,
            9,9,
            10,10,
            11,11,
            12
        };
    
    }

    void CreateEdge(Vertex a, Vertex b)
    {
         string key =
            a.GetInstanceID() < b.GetInstanceID()
            ? a.GetInstanceID() + "_" + b.GetInstanceID()
            : b.GetInstanceID() + "_" + a.GetInstanceID();

        if(edgeKeys.Contains(key))
        {
            return;
        }

        edgeKeys.Add(key);

        Vector3 center =
            (a.transform.position +
             b.transform.position) / 2f;

        Vector3 dir =
            b.transform.position -
            a.transform.position;

        float angle =
            Mathf.Atan2(dir.z, dir.x)
            * Mathf.Rad2Deg;

        Quaternion rotation =
            Quaternion.Euler
                (
                    0,
                    -angle,
                    0
                );

        GameObject edge =
            Instantiate
            (
                EdgePrefab,
                center,
                rotation
            );

        Edge edgeScript =
            edge.GetComponent<Edge>();

        edgeScript.vertexA = a;
        edgeScript.vertexB = b;

        a.connectedEdges.Add(edgeScript);
        b.connectedEdges.Add(edgeScript);
        allEdges.Add(edgeScript);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetBoardCenter()
    {
        float centerZ = 2 * zOffset;
        return new Vector3(0f, 0f, centerZ);
    }

        // 【追加】盤面の外周（海に面した辺）を自動検出し、9つの港を配置するアルゴリズム
    void CreateHarbors()
    {
        // 1. 外周の辺（vertexA と vertexB が共有するタイルが1枚だけの辺）を抽出
        List<Edge> coastalEdges = new List<Edge>();
        foreach (Edge edge in allEdges)
        {
            int sharedTiles = 0;
            foreach (HexTile tile in edge.vertexA.adjacentTiles)
            {
                if (edge.vertexB.adjacentTiles.Contains(tile))
                {
                    sharedTiles++;
                }
            }
            // 共有タイルが1枚だけなら、片側が海の「沿岸の辺」！
            if (sharedTiles == 1)
            {
                coastalEdges.Add(edge);
            }
        }

        // 2. 外周の辺を盤面の中心からの角度順にソート（ぐるっと1周順番に並べる）
        Vector3 boardCenter = GetBoardCenter();
        coastalEdges.Sort((a, b) =>
        {
            float angleA = Mathf.Atan2(a.transform.position.z - boardCenter.z, a.transform.position.x - boardCenter.x);
            float angleB = Mathf.Atan2(b.transform.position.z - boardCenter.z, b.transform.position.x - boardCenter.x);
            return angleA.CompareTo(angleB);
        });

        // 3. 公式ルールの港9種類（3:1が4つ、2:1が5つ）をシャッフルして用意
        List<HarborType> harbors = new List<HarborType>
        {
            HarborType.Generic3to1, HarborType.Generic3to1, HarborType.Generic3to1, HarborType.Generic3to1,
            HarborType.Wood2to1, HarborType.Brick2to1, HarborType.Sheep2to1, HarborType.Wheat2to1, HarborType.Ore2to1
        };
        for (int i = harbors.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            HarborType tmp = harbors[i];
            harbors[i] = harbors[rnd];
            harbors[rnd] = tmp;
        }

        // 4. 外周30辺のうち、3辺おき（インデックス 0, 3, 6, 10, 13, 16, 20, 23, 26）に均等配置！
        int[] harborIndices = { 0, 3, 6, 10, 13, 16, 20, 23, 26 };
        for (int i = 0; i < harbors.Count && i < harborIndices.Length; i++)
        {
            int edgeIdx = harborIndices[i];
            if (edgeIdx >= coastalEdges.Count) break;

            Edge targetEdge = coastalEdges[edgeIdx];
            HarborType type = harbors[i];

            // 辺の両端の頂点（Vertex）に港の能力を付与する
            targetEdge.vertexA.harborType = type;
            targetEdge.vertexB.harborType = type;

            // 見た目（harborPrefab）を少し海側（外側）にずらして生成
                        // 【修正】その辺に接している唯一のタイルの中心を基準にして、完璧に辺の垂直方向（海側）へ配置する
            if (harborPrefab != null)
            {
                // この辺（vertexAとvertexB）が共有している1枚のタイルを取得
                HexTile adjacentTile = null;
                foreach (HexTile tile in targetEdge.vertexA.adjacentTiles)
                {
                    if (targetEdge.vertexB.adjacentTiles.Contains(tile))
                    {
                        adjacentTile = tile;
                        break;
                    }
                }

                // タイル中心から辺の中心へ向かうベクトル（＝辺に対して完璧に90度垂直な海側の向き！）
                Vector3 outwardDir = (adjacentTile != null)
                    ? (targetEdge.transform.position - adjacentTile.transform.position).normalized
                    : (targetEdge.transform.position - boardCenter).normalized;
                outwardDir.y = 0f; // 高さがズレないように水平に固定

                Vector3 spawnPos = targetEdge.transform.position + outwardDir * 0.6f;
                Quaternion rot = Quaternion.LookRotation(-outwardDir); // 辺と完璧に平行になる向き
                GameObject harborObj = Instantiate(harborPrefab, spawnPos, rot);
                harborObj.name = $"Harbor_{type}";


                TMPro.TMP_Text label = harborObj.GetComponentInChildren<TMPro.TMP_Text>();
                if (label != null)
                {
                    label.text = GetHarborLabel(type);
                }
            }
        }
    }
    
        private string GetHarborLabel(HarborType type)
    {
        switch (type)
        {
            case HarborType.Generic3to1: return "3 : 1\nANY";
            case HarborType.Wood2to1:    return "2 : 1\nWOOD";
            case HarborType.Brick2to1:   return "2 : 1\nBRICK";
            case HarborType.Sheep2to1:   return "2 : 1\nSHEEP";
            case HarborType.Wheat2to1:   return "2 : 1\nWHEAT";
            case HarborType.Ore2to1:     return "2 : 1\nORE";
            default: return "";
        }
    }

}
