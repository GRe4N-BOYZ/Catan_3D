using UnityEngine;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance;
    public const int VictoryPointToWin = 5;

    public void Awake()
    {
        Instance = this;
    }
    public int GetVictoryPoints(Player player)
    {
        int total = 0;

        Vertex[] vertices =
            UnityEngine.Object.FindObjectsByType<Vertex>
            (
                FindObjectsSortMode.None
            );
        
        foreach(Vertex vertex in vertices)
        {
            if(vertex.building == null)
            {
                continue;
            }
            if(vertex.building.owner != player)
            {
                continue;
            }

            total += vertex.building.VictoryPoint;
        }
        return total;
    }

    public bool CheckVictory(Player player)
    {
        return GetVictoryPoints(player) >= VictoryPointToWin;
    }
}
