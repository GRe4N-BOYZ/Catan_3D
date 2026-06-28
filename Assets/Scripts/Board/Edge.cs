using UnityEngine;
using UnityEngine.UIElements;

public class Edge : MonoBehaviour
{
    public Vertex vertexA;
    public Vertex vertexB;
    public Road road;

    public bool BuildRoad(GameObject roadPrefab, Player player)
        {
            if(road != null)
            {
                Debug.Log("すでに街道が存在します");
                return false;
            }
            if(!PlayerManager.Instance.setupPhase && !CanBuildRoad(player))
            {
                Debug.Log("自分の村と接続していません");
                return false;
            }
            if(GameManager.Instance.currentState
            != GameManager.GameState.InitialRoad)
                {
                    return false;
                }

            if(PlayerManager.Instance.setupPhase)
                {
                    Vertex last =
                    GameManager.Instance.lastPlacedSettlement;
                    
                    if(vertexA != last && vertexB != last)
                    {
                        Debug.Log("直前の開拓地に接続してください");
                        return false;
                    }
                }
            

            GameObject obj =
                Instantiate
                (
                    roadPrefab,
                    transform.position,
                    transform.rotation
                );

            road = obj.GetComponent<Road>();
            road.owner = player;

            Renderer renderer =
                obj.GetComponentInChildren<Renderer>();

            if(renderer != null)
            {
                renderer.material.color = PlayerColorUtil.ToUnityColor(player.color);
            }
            
            PlayerManager.Instance.FinishInitialRoad();

            return true;
        }
    public bool CanBuildRoad(Player player)
    {
        if
        (vertexA.settlement != null &&
        vertexA.settlement.owner == player)
        {
            return true;
        }else if
        (vertexB.settlement != null &&
        vertexB.settlement.owner == player)
        {
            return true;
        }else
        {
            return false;
        }
    }

    private bool IsRoadConnectedAtVertex(Vertex vertex, Player player)
    {
        // 【分断ルール】もしその交差点に「自分以外の開拓地（敵の村）」があったら、道路は分断されて伸ばせない！
        if (vertex.settlement != null && vertex.settlement.owner != player)
        {
            return false; 
        }

        // 交差点に繋がっている道路がない（null）ならチェック不要
        if (vertex.connectedEdges == null) return false;

        // 交差点に繋がっている道路（Edge）を1本ずつ調べる
        foreach (Edge neighborEdge in vertex.connectedEdges)
        {
            // 自分自身のEdge（これから建てようとしている場所）は無視する
            if (neighborEdge == this) continue;

            // その隣のEdgeに「道路がすでに建っていて」、かつ「持ち主が自分」なら繋がっている！
            if (neighborEdge.road != null && neighborEdge.road.owner == player)
            {
                return true;
            }
        }

        return false;
    }
}
