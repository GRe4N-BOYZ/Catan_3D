using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class Vertex : MonoBehaviour
{
    public List<HexTile> adjacentTiles = new List<HexTile>();
    public List<Edge> connectedEdges = new List<Edge>();

    public Building building;
    public bool HasBuilding()
    {
        return building != null;
    }

    public bool BuildSettlement(GameObject settlementPrefab, Player player)
    {
        if(GameManager.Instance.currentState != GameManager.GameState.InitialSettlement)
        {
            return false;
        }
        if(PlayerManager.Instance.setupPhase)
        {
            if(!CanBuildInitialSettlement())
            {
                Debug.Log("ここには建設できません");
                return false;
            }
        } else
        {
            if(!CanBuildSettlement(player))
            {
                Debug.Log("自分の街道と接続していません");
                return false;
            }
            
            if(!player.CanAffordSettlement())
            {
                Debug.Log("資源が足りません");
                return false;
            }
        }

        building = CreateSettlement(settlementPrefab, player);

        GameManager.Instance.lastPlacedSettlement = this;
        
        if(!PlayerManager.Instance.setupPhase)
        {
            player.SpendSettlementCost();
            UIManager.Instance.UpdateAll();
        }
        
        GameManager.Instance.ChangeState
        (
            GameManager.GameState.InitialRoad
        );
        
        return true;
    }

    public bool UpgradeToCity(GameObject cityPrefab, Player player)
    {
        if(!CanUpgradeToCity(player))
        {
            Debug.Log("都市化できません");
            return false;
        }
        if(player.CanAffordCity())
        {
            Debug.Log("資源が足りません");
            return false;
        }
        CreateCity(cityPrefab, player);
        player.SpendCityCost();
        UIManager.Instance.UpdateAll();
        
        return true;
    }

    public bool HasAdjacentBuilding()
    {
        foreach(Edge edge in connectedEdges)
        {
            Vertex other =
            edge.vertexA == this
                ? edge.vertexB
                : edge.vertexA;
            
            if(other != null && other.HasBuilding())
            {
                return true;
            }
        }
        return false;
    }
 
    public bool CanBuildSettlement(Player player)
    {
        if(building != null) return false;
        if(HasAdjacentBuilding()) return false;

        if (connectedEdges == null) return false;

        foreach (Edge edge in connectedEdges)
            {
                if (edge.road != null && edge.road.owner == player)
                    {
                        return true;
                    }
            }
        return false;
    }
    
    public bool CanBuildInitialSettlement()
    {
        if(building != null) return false;

        if(HasAdjacentBuilding()) return false;
        
        return true;
    }

    public bool CanUpgradeToCity(Player player)
    {
        if(building == null) return false;
        if(building is City) return false;
        Settlement settlement = building as Settlement;
        if(settlement == null) return false;
        if(settlement.owner != player) return false;
        

        return true;
    }

    private Settlement CreateSettlement(GameObject prefab, Player player)
    {
        GameObject obj = Instantiate
        (
            prefab,
            transform.position,
            Quaternion.identity
        );

        Settlement settlement = obj.GetComponent<Settlement>();

        settlement.owner = player;

        Renderer renderer = obj.GetComponentInChildren<Renderer>();

        if(renderer != null)
        {
            renderer.material.color = PlayerColorUtil.ToUnityColor(player.color);
        }
        return settlement;
    }

    private City CreateCity(GameObject prefab, Player player)
    {
        Destroy(building.gameObject);
        GameObject obj = Instantiate
        (
            prefab,
            transform.position,
            Quaternion.identity
        );
        City city = obj.GetComponent<City>();
        city.owner = player;
        Renderer renderer = obj.GetComponentInChildren<Renderer>();

        if(renderer != null)
        {
            renderer.material.color = PlayerColorUtil.ToUnityColor(player.color);
        }
        building = city;
        return city;
    }
}
