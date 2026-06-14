using System.Collections.Generic;
using UnityEngine;

public class Vertex : MonoBehaviour
{
    public List<HexTile> adjacentTiles = new List<HexTile>();

    public Settlement settlement;

    public bool hasSettlement()
    {
        return settlement != null;
    }

    public bool BuildSettlement(GameObject settlementPrefab, Player player)
    {
        if(settlement != null)
        {
            return false;
        }

        GameObject obj =
            Instantiate
            (
                settlementPrefab,
                transform.position,
                Quaternion.identity
            );
        settlement = obj.GetComponent<Settlement>();

        settlement.owner = player;

        Renderer renderer =
            obj.GetComponentInChildren<Renderer>();

        if(renderer != null)
        {
            renderer.material.color = PlayerColorUtil.ToUnityColor(player.color);
        }

        return true;
    }
}
