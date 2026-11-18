using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour
{
    Camera mainCamera;
    public GameObject settlementPrefab;
    public GameObject roadPrefab;
    public GameObject cityPrefab;

    [FormerlySerializedAs("vertexLayer")]
    private LayerMask currentMask;
    private LayerMask settlementMask;
    private LayerMask roadMask;
    private LayerMask hexTileMask;

    public void Start()
    {
        mainCamera = Camera.main;
        PlayerManager.EnsureInstance();
        
        settlementMask = LayerMask.GetMask("Vertex");
        roadMask = LayerMask.GetMask("Edge");
        hexTileMask = LayerMask.GetMask("HexTile");

        currentMask = settlementMask;
    }

    public void Update()
    {
        if(GameManager.Instance.currentState == GameManager.GameState.GameEnd)
        {
            return;
        }
        if(PlayerManager.Instance == null)
        {
            PlayerManager.EnsureInstance();
        }

        if(PlayerManager.Instance == null) return;
        if(Mouse.current == null) return;
        if(mainCamera == null) return;

                // 【追加】盗賊移動フェーズ中は、レイヤーマスクをHexTileに固定する
        if (GameManager.Instance.currentState == GameManager.GameState.MoveRobber)
        {
            currentMask = hexTileMask;
        }
        else if(!PlayerManager.Instance.setupPhase)
        {
            switch(GameManager.Instance.currentBuildMode)
            {
                case BuildMode.Settlement:
                    currentMask = settlementMask;
                    break;
                
                case BuildMode.Road:
                    currentMask = roadMask;
                    break;

                case BuildMode.City:
                    currentMask = settlementMask;
                    break;
                
                default:
                    currentMask = 0;
                    break;
            }
        } else
        {
            if(GameManager.Instance.currentState 
                == GameManager.GameState.InitialSettlement)
            {
                currentMask = settlementMask;
            } else
            {
                currentMask = roadMask;
            }
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

                // 【追加】盗賊移動フェーズなら、当たったタイルへ盗賊を移動させる
                if (GameManager.Instance.currentState == GameManager.GameState.MoveRobber)
                {
                    HexTile tile = hit.collider.GetComponentInParent<HexTile>();
                    if (tile != null)
                    {
                        RobberManager.Instance?.MoveRobberTo(tile);
                    }
                    return; // 盗賊移動が終わったらここで処理終了
                }

                Vertex vertex = hit.collider.GetComponentInParent<Vertex>();
                Edge edge = hit.collider.GetComponentInParent<Edge>();

                if(vertex != null)
                {
                    if(PlayerManager.Instance.setupPhase)
                    {
                        vertex.BuildSettlement
                        (
                            settlementPrefab,
                            PlayerManager.Instance.currentPlayer
                        );
                    } else
                    {
                        switch(GameManager.Instance.currentBuildMode)
                        {
                            case BuildMode.Settlement:
                                vertex.BuildSettlement
                                (
                                    settlementPrefab,
                                    PlayerManager.Instance.currentPlayer
                                );
                                break;
                            
                            case BuildMode.City:
                                vertex.UpgradeToCity
                                (
                                    cityPrefab,
                                    PlayerManager.Instance.currentPlayer
                                );
                                break;
                        }
                    }
                }
                else if(edge != null)
                {
                    if(PlayerManager.Instance.setupPhase)
                    {
                        edge.BuildRoad
                        (
                            roadPrefab,
                            PlayerManager.Instance.currentPlayer
                        );
                    } else if(GameManager.Instance.currentBuildMode == BuildMode.Road)
                    {
                        edge.BuildRoad
                        (
                            roadPrefab,
                            PlayerManager.Instance.currentPlayer
                        );
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
}
