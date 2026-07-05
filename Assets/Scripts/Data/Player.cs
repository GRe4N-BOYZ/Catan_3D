using Unity.Mathematics;

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
}
