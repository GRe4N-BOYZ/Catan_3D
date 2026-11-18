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
        
        // 【追加】7が出たら盗賊移動フェーズへ移行して処理を止める
        if (lastDiceResult == 7)
        {
            Debug.Log("盗賊（7）が出ました！移動先のタイルをクリックしてください。");
            GameManager.Instance.ChangeState(GameManager.GameState.MoveRobber);
            return;
        }

        // 7以外なら通常通り資源を配布してPlayerActionへ
        ResourceManager.Instance.DistributeResources(lastDiceResult);
        UIManager.Instance.UpdateResourceUI();

        GameManager.Instance.ChangeState
        (
            GameManager.GameState.PlayerAction
        );
    }
}