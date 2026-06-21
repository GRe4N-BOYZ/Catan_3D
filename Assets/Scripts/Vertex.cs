using System.Collections.Generic;
using UnityEngine;

public class Vertex : MonoBehaviour
{
    public List<HexTile> adjacentTiles = new List<HexTile>();
    public List<Edge> connectedEdges = new List<Edge>();

    public Settlement settlement;
    public bool hasSettlement()
    {
        return settlement != null;
    }

    public bool BuildSettlement(GameObject settlementPrefab, Player player)
    {
        if(GameManager.Instance.currentState
            != GameManager.GameState.InitialSettlement)
        {
            return false;
        }

        if(settlement != null)
        {
            return false;
        }

        if (HasAdjacentSettlement())
            {
                Debug.Log("隣に開拓地があります");
                return false;
            }
        
        if(!PlayerManager.Instance.setupPhase && !CanBuildSettlement(player))
        {
            Debug.Log("自分の街道と接続していません");
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
        GameManager.Instance.lastPlacedSettlement = this;
        GameManager.Instance.currentState = GameManager.GameState.InitialRoad;
        Renderer renderer = obj.GetComponentInChildren<Renderer>();

        if(renderer != null)
        {
            renderer.material.color = PlayerColorUtil.ToUnityColor(player.color);
        }
        GameManager.Instance.currentState =
            GameManager.GameState.InitialRoad;

        return true;
    }

    public bool HasAdjacentSettlement()
    {
        foreach(Edge edge in connectedEdges)
        {
            Vertex other =
            edge.vertexA == this
                ? edge.vertexB
                : edge.vertexA;
            
            if(other != null && other.hasSettlement())
            {
                return true;
            }
        }
        return false;
    }

    public bool CanBuildSettlement(Player player)
        {
            // 繋がっている道路のリストが空っぽなら、当然建てられない
            if (connectedEdges == null) return false;

            // 繋がっている道路（Edge）を1本ずつ順番にチェックしていく
            foreach (Edge edge in connectedEdges)
                {
                    // その道路に「道路がすでに建っていて」、かつ「持ち主が自分」ならOK！
                    if (edge.road != null && edge.road.owner == player)
                        {
                            return true; // 1本でも見つかれば、その時点で建築可能（即終了）
                        }
                }

            // 全部調べても自分の道路が1本もなかったらダメ
            return false;
        }

}
