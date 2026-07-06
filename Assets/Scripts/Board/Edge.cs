using UnityEngine;
using UnityEngine.UIElements;

public class Edge : MonoBehaviour
{
    public Vertex vertexA;
    public Vertex vertexB;
    public Road road;

    public bool BuildRoad(GameObject roadPrefab, Player player)
    {
        if (PlayerManager.Instance.setupPhase)
        {
            if (!CanBuildInitialRoad())
            {
                Debug.Log("直前の開拓地に接続してください");
                return false;
            }
            if (GameManager.Instance.currentState != GameManager.GameState.InitialRoad)
            {
                return false;
            }
        } else
        {
            if (!CanBuildRoad(player))
            {
                Debug.Log("ここには道路を建てられません");
                return false;
            }
                
            if (!player.CanAffordRoad())
            {
                Debug.Log("資源が足りません");
                return false;
            }
            if (GameManager.Instance.currentState != GameManager.GameState.PlayerAction)
            {
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
            
        if (PlayerManager.Instance.setupPhase)
        {
            PlayerManager.Instance.FinishInitialRoad();
        } else
        {
            player.SpendRoadCost();
            UIManager.Instance.UpdateAll();
            GameManager.Instance.ChangeBuildMode(BuildMode.None);
        }

        return true;
    }

    public bool CanBuildRoad(Player player)
    {
        //街道あるか
        if(road != null) return false;
        
        //村から接続
        if(vertexA.building != null &&
        vertexA.building.owner == player) return true;
    
        if
        (vertexB.building != null &&
        vertexB.building.owner == player) return true;

        // 道路から接続
        if (IsRoadConnectedAtVertex(vertexA, player)) return true;

        if (IsRoadConnectedAtVertex(vertexB, player)) return true;
        
        return false;
    }

    private bool IsRoadConnectedAtVertex(Vertex vertex, Player player)
    {
        // 【分断ルール】もしその交差点に「自分以外の開拓地（敵の村）」があったら、道路は分断されて伸ばせない！
        if (vertex.building != null && vertex.building.owner != player)
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

    public bool CanBuildInitialRoad()
    {
        Vertex last = GameManager.Instance.lastPlacedSettlement;

        return vertexA == last ||
               vertexB == last;
    }
}
