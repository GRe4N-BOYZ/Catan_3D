using UnityEngine;

public class TradeManager : MonoBehaviour
{
    public static TradeManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // プレイヤーが持っている港から「最安のレート（2, 3, 4）」を計算する
    public int GetBestTradeCost(Player player, ResourceType giveType)
    {
        int bestRate = 4; // 基本レートは 4:1

        Vertex[] allVertices = FindObjectsByType<Vertex>(FindObjectsSortMode.None);
        foreach (Vertex v in allVertices)
        {
            if (v.building != null && v.building.owner == player)
            {
                if (v.harborType == HarborType.Generic3to1 && bestRate > 3)
                {
                    bestRate = 3;
                }
                else if (IsMatchingSpecialistHarbor(v.harborType, giveType))
                {
                    return 2;
                }
            }
        }
        return bestRate;
    }

    // 指定したレート（4, 3, 2）をそのプレイヤーが使う権利があるかチェック
    public bool IsRateAvailable(Player player, ResourceType giveType, int targetRate)
    {
        if (targetRate == 4) return true; // 4:1銀行貿易は誰でもいつでも使える

        int bestRate = GetBestTradeCost(player, giveType);
        
        if (targetRate == 2) return bestRate == 2;

        if (targetRate == 3)
        {
            Vertex[] allVertices = FindObjectsByType<Vertex>(FindObjectsSortMode.None);
            foreach (Vertex v in allVertices)
            {
                if (v.building != null && v.building.owner == player && v.harborType == HarborType.Generic3to1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // 交換可能かどうかを判定する（customRate引数付き）
    public bool CanTradeWithBank(Player player, ResourceType giveType, ResourceType receiveType, int customRate = -1)
    {
        if (GameManager.Instance.currentState != GameManager.GameState.PlayerAction) return false;
        if (giveType == receiveType) return false;
        if (giveType == ResourceType.Desert || receiveType == ResourceType.Desert) return false;

        int cost = (customRate > 0) ? customRate : GetBestTradeCost(player, giveType);

        if (!IsRateAvailable(player, giveType, cost)) return false;

        return player.GetResourceCount(giveType) >= cost;
    }

    // 実際に銀行との資源交換を実行する（customRate引数付き）
    public bool ExecuteBankTrade(Player player, ResourceType giveType, ResourceType receiveType, int customRate = -1)
    {
        int cost = (customRate > 0) ? customRate : GetBestTradeCost(player, giveType);

        if (!CanTradeWithBank(player, giveType, receiveType, cost))
        {
            Debug.LogWarning("Trade condition not met.");
            return false;
        }

        player.AddResource(giveType, -cost);
        player.AddResource(receiveType, 1);

        Debug.Log($"Trade executed! {player.name} paid {cost} {giveType} for 1 {receiveType}.");
        UIManager.Instance?.UpdateResourceUI();
        return true;
    }

    private bool IsMatchingSpecialistHarbor(HarborType harbor, ResourceType resource)
    {
        switch (resource)
        {
            case ResourceType.Wood: return harbor == HarborType.Wood2to1;
            case ResourceType.Brick: return harbor == HarborType.Brick2to1;
            case ResourceType.Sheep: return harbor == HarborType.Sheep2to1;
            case ResourceType.Wheat: return harbor == HarborType.Wheat2to1;
            case ResourceType.Ore: return harbor == HarborType.Ore2to1;
            default: return false;
        }
    }
}
