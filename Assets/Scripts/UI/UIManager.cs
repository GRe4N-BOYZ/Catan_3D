using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Texts")]
    public TMP_Text playerText;
    public TMP_Text resourceText;
    public TMP_Text diceText;
    public TMP_Text phaseText;

    public Button settlementButton;
    public Button roadButton;
    public Button cityButton;
    public Button endTurnButton;

    private void Awake()
    {
        Instance = this;
    }

    public void UpdateAll()
    {
        UpdatePlayerUI();
        UpdateResourceUI();
        UpdateDiceUI();
        UpdatePhaseUI();
    }

    private void UpdateButton()
    {
        settlementButton.onClick.AddListener(SetSettlementMode);
        roadButton.onClick.AddListener(SetRoadMode);
        cityButton.onClick.AddListener(SetCityMode);
        endTurnButton.onClick.AddListener(EndTurn);
    }

    public void UpdatePlayerUI()
    {
        if(PlayerManager.Instance == null)
            return;

        Player player =
            PlayerManager.Instance.currentPlayer;

        playerText.text =
            $"Current Player\n{player.name}";
    }

    public void UpdateResourceUI()
    {
        if(PlayerManager.Instance == null)
            return;

        Player player =
            PlayerManager.Instance.currentPlayer;

        resourceText.text =
            $"Wood : {player.wood}\n" +
            $"Brick : {player.brick}\n" +
            $"Sheep : {player.sheep}\n" +
            $"Wheat : {player.wheat}\n" +
            $"Ore : {player.ore}";
    }

    public void UpdateDiceUI()
    {
        diceText.text = 
            $"Dice : {DiceManager.Instance.lastDiceResult}";
    }

    /*public void UpdatePhaseUI()
    {
        phaseText.text = 
            $"Phase : {GameManager.Instance.currentState}";
    }*/
    public void UpdatePhaseUI()
    {
        if (GameManager.Instance == null) return;

        var state = GameManager.Instance.currentState;
        phaseText.text = $"Phase : {state}";

        // 【追加】通常行動フェーズ（PlayerAction）の時だけ、建設ボタンやEndTurnボタンを押せるようにする
        bool isPlayerAction = (state == GameManager.GameState.PlayerAction);
        
        if (settlementButton != null) settlementButton.interactable = isPlayerAction;
        if (roadButton != null) roadButton.interactable = isPlayerAction;
        if (cityButton != null) cityButton.interactable = isPlayerAction;
        if (endTurnButton != null) endTurnButton.interactable = isPlayerAction;
    }


    public void SetSettlementMode()
    {
        if(GameManager.Instance.currentState
            != GameManager.GameState.PlayerAction) return;
        GameManager.Instance.ChangeBuildMode
        (
            BuildMode.Settlement
        );
    } 

    public void SetRoadMode()
    {
        if(GameManager.Instance.currentState
            != GameManager.GameState.PlayerAction) return;
        GameManager.Instance.ChangeBuildMode
        (
            BuildMode.Road
        );
    }

    public void SetCityMode()
    {
        if(GameManager.Instance.currentState
            != GameManager.GameState.PlayerAction) return;
        GameManager.Instance.ChangeBuildMode
        (
            BuildMode.City
        );
    }

    public void EndTurn()
    {
        // 【追加】PlayerAction以外の状態（初期配置中やダイス振り前など）なら何もしない
        if (GameManager.Instance.currentState != GameManager.GameState.PlayerAction)
        {
            Debug.LogWarning("初期配置またはダイスロールが完了していないため、ターンを終了できません。");
            return;
        }
        PlayerManager.Instance.EndTurn();
        Debug.Log("Turn End");
    }

    private void Start()
    {
        UpdateAll();
        UpdateButton();
    }
}