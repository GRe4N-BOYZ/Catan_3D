using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance;
    public int lastDiceResult;
    private void Awake()
    {
        Instance = this;
    }
    public void RollDice()
    {
        int dice1 = Random.Range(1, 7);
        int dice2 = Random.Range(1, 7);

        lastDiceResult = dice1 + dice2;

        Debug.Log($"Dice : {dice1} + {dice2} = {lastDiceResult}");
        ResourceManager.Instance.DistributeResources(lastDiceResult);

        UIManager.Instance.UpdateDiceUI();
        UIManager.Instance.UpdateResourceUI();

        GameManager.Instance.ChangeState
        (
            GameManager.GameState.PlayerAction
        );
    }
}