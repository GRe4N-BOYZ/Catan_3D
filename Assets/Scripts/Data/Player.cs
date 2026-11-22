using System;
using UnityEngine;

public class Player
{
    public string name;
    public PlayerColor color;
    public int settlementCount;
    public int wood;
    public int brick;
    public int wheat;
    public int sheep;
    public int ore;

    public bool CanAffordRoad()
    {
        return wood >= 1 &&
            brick >= 1;
    }
    public void SpendRoadCost()
    {
        wood --;
        brick --;
    }

    public bool CanAffordSettlement()
    {
        return wood >= 1 &&
               brick >= 1 &&
               sheep >= 1 &&
               wheat >= 1;
    }

    public void SpendSettlementCost()
    {
        wood--;
        brick--;
        sheep--;
        wheat--;
    }

    public bool CanAffordCity()
    {
        return wheat >= 2 &&
               ore >= 3;
    }

    public void SpendCityCost()
    {
        wheat -= 2;
        ore -= 3;
    }

    public void DebugAddResources(int amount)
    {
        wood += amount;
        brick += amount;
        sheep += amount;
        wheat += amount;
        ore += amount;

        Debug.Log(
        $"{name} Debug Resources +{amount}"
    );
    }

        // 【追加】指定した種類の資源の現在の所持数を返す
    public int GetResourceCount(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Wood: return wood;
            case ResourceType.Brick: return brick;
            case ResourceType.Sheep: return sheep;
            case ResourceType.Wheat: return wheat;
            case ResourceType.Ore: return ore;
            default: return 0;
        }
    }

    // 【追加】指定した種類の資源を増減させる（マイナスを渡せば消費になる）
    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Wood: wood += amount; break;
            case ResourceType.Brick: brick += amount; break;
            case ResourceType.Sheep: sheep += amount; break;
            case ResourceType.Wheat: wheat += amount; break;
            case ResourceType.Ore: ore += amount; break;
        }
    }

}
