using UnityEngine;

public class Edge : MonoBehaviour
{
    public Vertex vertexA;
    public Vertex vertexB;
    public Road road;

    public bool BuildRoad(GameObject roadPrefab, Player player)
        {
            if(road != null)
            {
                return false;
            }

            GameObject obj =
                Instantiate
                (
                    roadPrefab,
                    transform.position,
                    transform.rotation
                );

            road =
                obj.GetComponent<Road>();

            road.owner = player;

            Renderer renderer =
                obj.GetComponentInChildren<Renderer>();

            if(renderer != null)
            {
                renderer.material.color = PlayerColorUtil.ToUnityColor(player.color);
            }

            return true;
        }
}
