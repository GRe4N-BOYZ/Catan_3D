using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void DistributeResources(int diceResult)
    {
        HexTile[] allTiles =
            FindObjectsByType<HexTile>
            (
                FindObjectsSortMode.None
            );

        foreach (HexTile tile in allTiles)
        {
            // 出目が違う
            if (tile.numberToken != diceResult)
            {
                continue;
            }

            // 盗賊がいる
            if (tile.hasRobber)
            {
                continue;
            }

            GiveTileResources(tile);
        }
        UIManager.Instance.UpdateAll();
    }

    private void GiveTileResources(HexTile tile)
    {
        foreach (Vertex vertex in tile.adjacentVertices)
        {
            if (vertex == null) continue;
            if (vertex.building == null) continue;

            Player owner = vertex.building.owner;
            
            int amount = 1;
            if(vertex.building is City) amount = 2;

            GiveResource
            (
                owner,
                tile.resourceType,
                amount
            );
        }
    }

    private void GiveResource
    (
        Player player,
        ResourceType type,
        int amount
    )
    {
        switch (type)
        {
            case ResourceType.Wood:
                player.wood += amount;
                break;

            case ResourceType.Brick:
                player.brick += amount;
                break;

            case ResourceType.Sheep:
                player.sheep += amount;
                break;

            case ResourceType.Wheat:
                player.wheat += amount;
                break;

            case ResourceType.Ore:
                player.ore += amount;
                break;

            case ResourceType.Desert:
                return;
        }

        Debug.Log
        (
            player.name +
            " received " +
            type
        );

        Debug.Log
        (
            $"Wood:{player.wood} " +
            $"Brick:{player.brick} " +
            $"Sheep:{player.sheep} " +
            $"Wheat:{player.wheat} " +
            $"Ore:{player.ore}"
        );
    }
}