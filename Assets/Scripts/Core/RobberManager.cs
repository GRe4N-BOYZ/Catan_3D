using System.Collections.Generic;
using UnityEngine;

public class RobberManager : MonoBehaviour
{
    public static RobberManager Instance;

    [Header("Robber Visual")]
    public GameObject robberPrefab;
    private GameObject currentRobberObject;
    private HexTile currentTile;

    private void Awake()
    {
        Instance = this;
    }

    // 初期配置（砂漠タイルに盗賊をスポーンさせる）
    public void InitializeRobber(HexTile desertTile)
    {
        currentTile = desertTile;
        currentTile.hasRobber = true;

        if (robberPrefab != null)
        {
            Vector3 spawnPos = desertTile.transform.position + new Vector3(0f, 0.5f, 0f);
            currentRobberObject = Instantiate(robberPrefab, spawnPos, Quaternion.identity);
        }
    }

    // タイルクリック時に呼ばれる移動＆強奪処理
    public void MoveRobberTo(HexTile targetTile)
    {
        if (targetTile == currentTile)
        {
            Debug.LogWarning("現在盗賊がいるタイルとは別のタイルを選んでください！");
            return;
        }

        // 1. フラグの更新
        if (currentTile != null) currentTile.hasRobber = false;
        currentTile = targetTile;
        currentTile.hasRobber = true;

        // 2. 3Dモデルの移動
        if (currentRobberObject != null)
        {
            currentRobberObject.transform.position = targetTile.transform.position + new Vector3(0f, 0.5f, 0f);
        }

        // 3. 隣接する他プレイヤーから資源を1枚強奪
        StealResourceFromAdjacentPlayer(targetTile);

        // 4. 通常行動フェーズへ移行
        GameManager.Instance.ChangeState(GameManager.GameState.PlayerAction);
    }

    private void StealResourceFromAdjacentPlayer(HexTile tile)
    {
        Player currentPlayer = PlayerManager.Instance.currentPlayer;
        HashSet<Player> targetPlayers = new HashSet<Player>();

        foreach (Vertex v in tile.adjacentVertices)
        {
            if (v.building != null && v.building.owner != null && v.building.owner != currentPlayer)
            {
                targetPlayers.Add(v.building.owner);
            }
        }

        if (targetPlayers.Count == 0)
        {
            Debug.Log("奪える相手プレイヤーが隣接していません。");
            return;
        }

        List<Player> candidates = new List<Player>(targetPlayers);
        Player victim = candidates[Random.Range(0, candidates.Count)];

        List<ResourceType> availableResources = new List<ResourceType>();
        for (int i = 0; i < victim.wood; i++) availableResources.Add(ResourceType.Wood);
        for (int i = 0; i < victim.brick; i++) availableResources.Add(ResourceType.Brick);
        for (int i = 0; i < victim.sheep; i++) availableResources.Add(ResourceType.Sheep);
        for (int i = 0; i < victim.wheat; i++) availableResources.Add(ResourceType.Wheat);
        for (int i = 0; i < victim.ore; i++) availableResources.Add(ResourceType.Ore);

        if (availableResources.Count == 0)
        {
            Debug.Log($"{victim.name} は資源を1枚も持っていませんでした！");
            return;
        }

        ResourceType stolen = availableResources[Random.Range(0, availableResources.Count)];
        
        AddResourceToPlayer(victim, stolen, -1);
        AddResourceToPlayer(currentPlayer, stolen, 1);

        Debug.Log($"盗賊発動！ {victim.name} から {stolen} を1枚奪いました！");
        UIManager.Instance?.UpdateResourceUI();
    }

    private void AddResourceToPlayer(Player p, ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Wood: p.wood += amount; break;
            case ResourceType.Brick: p.brick += amount; break;
            case ResourceType.Sheep: p.sheep += amount; break;
            case ResourceType.Wheat: p.wheat += amount; break;
            case ResourceType.Ore: p.ore += amount; break;
        }
    }
}
