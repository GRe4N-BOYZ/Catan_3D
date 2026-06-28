using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public ResourceType resourceType;
    public int numberToken;
    public int diceNumber;
    public bool hasRobber;
    public List<Vertex> adjacentVertices = new List<Vertex>();

    void Start()
    {
        UpdateColor();
    }

    void UpdateColor()
    {
        Renderer renderer =
            GetComponentInChildren<Renderer>();

        switch(resourceType)
        {
            case ResourceType.Wood:
                renderer.material.color = Color.green;
                break;
            
            case ResourceType.Brick:
                renderer.material.color = new Color(0.7f, 0.3f, 0.2f);
                break;
            
            case ResourceType.Sheep:
                renderer.material.color = new Color(0.5f, 1f, 0.5f);
                break;

            case ResourceType.Wheat:
                renderer.material.color = Color.yellow;
                break;

            case ResourceType.Ore:
                renderer.material.color = Color.gray;
                break;

            case ResourceType.Desert:
                renderer.material.color = new Color(1f, 0.9f, 0.5f);
                break;
        }
    }
}
