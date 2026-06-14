using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class HexBoard : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject hexPrefab;
    public GameObject vertexPrefab;
    public GameObject EdgePrefab;
    public GameObject settlementPrefab;

    Dictionary<Vector3, Vertex> vertices =
    new Dictionary<Vector3, Vertex>();

    List<Vertex> allVertices = new List<Vertex>();
    HashSet<string> edgeKeys = new HashSet<string>();

    float xOffset = Mathf.Sqrt(3);
    float zOffset = 1.5f;

    void Start()
    {
        CreateHexBoard();
    }

    void CreateHexBoard()
    {
        List<ResourceType> resources =
            CreateResources();

        List<int> numbers =
            CreateNumbers();

        int[] rows = { 3, 4, 5, 4, 3 };

        for (int row = 0; row < rows.Length; row++)
        {
            int count = rows[row];
            
            float startX = -(count -1) * xOffset / 2f; // Center the row

            for (int col = 0; col < count; col++)
            {
                // xPos に rowOffset を加える
                float xPos = startX + (col * xOffset);
                float zPos = row * zOffset;

                Vector3 pos = new Vector3(xPos, 0, zPos);
                
                GameObject tileObject = Instantiate(hexPrefab, pos, Quaternion.identity);

                HexTile tile = tileObject.GetComponent<HexTile>();
                CreateVertices(pos, tile);

                int index = Random.Range(0, resources.Count);
                ResourceType resource = resources[index];
                tile.resourceType = resource;
                resources.RemoveAt(index);

                if(resource == ResourceType.Desert)
                {
                    tile.hasRobber = true;
                }
                else
                {
                    int numberIndex =
                        Random.Range(0, numbers.Count);
                    
                    tile.numberToken =
                        numbers[numberIndex];
                    
                    numbers.RemoveAt(numberIndex);
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
        /*Debug.Log(
        Vector3.Distance(
        a.transform.position,
        b.transform.position
        )
        );*/

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
