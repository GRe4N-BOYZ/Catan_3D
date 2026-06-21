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
            if (tile.diceNumber != diceResult)
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
    }

    private void GiveTileResources(HexTile tile)
    {
        foreach (Vertex vertex in tile.adjacentVertices)
        {
            if (vertex == null)
            {
                continue;
            }

            if (vertex.settlement == null)
            {
                continue;
            }

            Player owner =
                vertex.settlement.owner;

            GiveResource
            (
                owner,
                tile.resourceType
            );
        }
    }

    private void GiveResource
    (
        Player player,
        ResourceType type
    )
    {
        switch (type)
        {
            case ResourceType.Wood:
                player.wood++;
                break;

            case ResourceType.Brick:
                player.brick++;
                break;

            case ResourceType.Sheep:
                player.sheep++;
                break;

            case ResourceType.Wheat:
                player.wheat++;
                break;

            case ResourceType.Ore:
                player.ore++;
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