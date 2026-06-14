using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    Camera mainCamera;
    public GameObject settlementPrefab;
    public GameObject roadPrefab;

    [FormerlySerializedAs("vertexLayer")]
    public LayerMask BuildLayer;

    void Start()
    {
        mainCamera = Camera.main;
        PlayerManager.EnsureInstance();
        EnsureBuildLayerIncludesBuildObjects();
        Debug.Log("BuildLayer value: " + BuildLayer.value);
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

        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray =
                mainCamera.ScreenPointToRay
                (
                    Mouse.current.position.ReadValue()
                );

            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);

            if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, BuildLayer))
            {
                Debug.Log("Hit: " + hit.collider.name);

                Vertex vertex = hit.collider.GetComponentInParent<Vertex>();
                Edge edge = hit.collider.GetComponentInParent<Edge>();

                if(vertex != null)
                {
                    bool success = vertex.BuildSettlement
                    (
                        settlementPrefab,
                        PlayerManager.Instance.currentPlayer
                    );

                    if(success)
                    {
                        PlayerManager.Instance.NextPlayer();
                    }
                }
                else if(edge != null)
                {
                    bool success = edge.BuildRoad
                    (
                        roadPrefab,
                        PlayerManager.Instance.currentPlayer
                    );

                    if(success)
                    {
                        PlayerManager.Instance.NextPlayer();
                    }
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

        BuildLayer |= buildObjectLayers;
    }
}
