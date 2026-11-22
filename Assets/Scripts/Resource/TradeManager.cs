using UnityEngine;

public class TradeManager : MonoBehaviour
{
    public static TradeManager Instance;

    private void Awake()
    {
        Instance = this;
    }

        // 【完成版】プレイヤーが所有する港をすべて調べ、最もお得な交換レート（2, 3, 4）を返す
    public int GetTradeCost(Player player, ResourceType giveType)
    {
        int bestRate = 4; // 基本レートは 4:1

        Vertex[] allVertices = FindObjectsByType<Vertex>(FindObjectsSortMode.None);
        foreach (Vertex v in allVertices)
        {
            // この頂点に自分の建物（開拓地または都市）が建っているか？
            if (v.building != null && v.building.owner == player)
            {
                // 3:1 一般港を持っていればレートを 3 に下げる
                if (v.harborType == HarborType.Generic3to1 && bestRate > 3)
                {
                    bestRate = 3;
                }
                // 渡そうとしている資源に対応する 2:1 専門港を持っていれば最安の 2 にする
                else if (IsMatchingSpecialistHarbor(v.harborType, giveType))
                {
                    return 2; // 2が最小なので即確定して返す
                }
            }
        }

        return bestRate;
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


    // 交換可能かどうかを判定する
    public bool CanTradeWithBank(Player player, ResourceType giveType, ResourceType receiveType)
    {
        // 1. 通常行動フェーズ（PlayerAction）以外では貿易不可
        if (GameManager.Instance.currentState != GameManager.GameState.PlayerAction)
        {
            return false;
        }

        // 2. 同じ種類の資源同士（木を払って木をもらう等）は交換できない
        if (giveType == receiveType)
        {
            return false;
        }

        // 3. 砂漠は資源ではないので対象外
        if (giveType == ResourceType.Desert || receiveType == ResourceType.Desert)
        {
            return false;
        }

        // 4. 必要な枚数（4枚）を持っているかチェック
        int requiredAmount = GetTradeCost(player, giveType);
        return player.GetResourceCount(giveType) >= requiredAmount;
    }

    // 実際に銀行との資源交換を実行する
    public bool ExecuteBankTrade(Player player, ResourceType giveType, ResourceType receiveType)
    {
        if (!CanTradeWithBank(player, giveType, receiveType))
        {
            Debug.LogWarning("貿易の条件を満たしていません（資源不足、または同じ資源を選択しています）。");
            return false;
        }

        int cost = GetTradeCost(player, giveType);

        // 資源の支払いと獲得
        player.AddResource(giveType, -cost);
        player.AddResource(receiveType, 1);

        Debug.Log($"貿易成立！ {player.name} は {giveType} を {cost}枚支払い、{receiveType} を 1枚獲得しました。");

        // 左上のプレイヤー資源表示UIを最新に更新
        UIManager.Instance?.UpdateResourceUI();
        return true;
    }
}
