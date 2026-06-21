using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    Camera mainCamera;
    public GameObject settlementPrefab;
    public GameObject roadPrefab;

    [FormerlySerializedAs("vertexLayer")]
    private LayerMask BuildMask;
    private LayerMask currentMask;
    private LayerMask settlementMask;
    private LayerMask roadMask;

    void Start()
    {
        mainCamera = Camera.main;
        PlayerManager.EnsureInstance();
        
        settlementMask = LayerMask.GetMask("Vertex");
        roadMask = LayerMask.GetMask("Edge");

        currentMask = settlementMask;
    }

    void Update()
    {
        if(PlayerManager.Instance == null)
        {
            PlayerManager.EnsureInstance();
        }

        if(PlayerManager.Instance == null) return;
        if(Mouse.current == null) return;
        if(mainCamera == null) return;

        if(GameManager.Instance.currentState == GameManager.GameState.InitialSettlement)
        {
            currentMask = settlementMask;
        }
        else if(GameManager.Instance.currentState == GameManager.GameState.InitialRoad)
        {
            currentMask = roadMask;
        }

        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray =
                mainCamera.ScreenPointToRay
                (
                    Mouse.current.position.ReadValue()
                );

            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

            if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, currentMask))
            {
                Debug.Log("Hit: " + hit.collider.name);

                Vertex vertex = hit.collider.GetComponentInParent<Vertex>();
                Edge edge = hit.collider.GetComponentInParent<Edge>();

                if(vertex != null)
                {
                    Debug.Log
                    (
                        "connectedEdges = " +
                        vertex.connectedEdges.Count
                    );

                    /*bool success = vertex.BuildSettlement
                    (
                        settlementPrefab,
                        PlayerManager.Instance.currentPlayer
                    );

                    if(success)
                    {
                        PlayerManager.Instance.NextPlayer();
                    }*/
                    if(vertex != null)
                        {
                            vertex.BuildSettlement
                            (
                                settlementPrefab,
                                PlayerManager.Instance.currentPlayer
                            );
                        }
                }
                else if(edge != null)
                {
                    edge.BuildRoad
                    (
                        roadPrefab,
                        PlayerManager.Instance.currentPlayer
                    );
                }
                else
                {
                    Debug.Log("Hit object is not Vertex or Edge: " + hit.collider.name);
                }
            }
            else
            {
                Debug.Log("No hit on Vertex/Edge layer.");
            }
        }
    }

    void EnsureBuildLayerIncludesBuildObjects()
    {
        int buildObjectLayers =
            LayerMask.GetMask("Vertex", "Edge");

        BuildMask |= buildObjectLayers;
    }
}
