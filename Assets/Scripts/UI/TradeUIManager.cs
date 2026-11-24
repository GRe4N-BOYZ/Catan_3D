using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject tradePanel;
    public TMP_Dropdown giveDropdown;
    public TMP_Dropdown receiveDropdown;
    public TMP_Dropdown rateDropdown; // レート選択ドロップダウン
    public TMP_Text rateInfoText;
    public Button confirmTradeButton;

    private readonly ResourceType[] resourceOptions = new ResourceType[]
    {
        ResourceType.Wood,
        ResourceType.Brick,
        ResourceType.Sheep,
        ResourceType.Wheat,
        ResourceType.Ore
    };

    private void Start()
    {
        if (tradePanel != null) tradePanel.SetActive(false);

        if (giveDropdown != null) giveDropdown.onValueChanged.AddListener(_ => RefreshTradeUI());
        if (receiveDropdown != null) receiveDropdown.onValueChanged.AddListener(_ => RefreshTradeUI());
        if (rateDropdown != null) rateDropdown.onValueChanged.AddListener(_ => RefreshTradeUI());
        if (confirmTradeButton != null) confirmTradeButton.onClick.AddListener(OnConfirmTradeClicked);
    }

    public void ToggleTradePanel()
    {
        if (tradePanel == null) return;

        if (!tradePanel.activeSelf && GameManager.Instance.currentState != GameManager.GameState.PlayerAction)
        {
            Debug.LogWarning("Trade is only available during PlayerAction.");
            return;
        }

        tradePanel.SetActive(!tradePanel.activeSelf);

        if (tradePanel.activeSelf)
        {
            if (rateDropdown != null) rateDropdown.value = 0;
            RefreshTradeUI();
        }
    }

    private int GetSelectedRate(Player player, ResourceType giveType)
    {
        if (rateDropdown == null) return TradeManager.Instance.GetBestTradeCost(player, giveType);

        switch (rateDropdown.value)
        {
            case 1: return 4; // 4:1 (Bank)
            case 2: return 3; // 3:1 (Harbor)
            case 3: return 2; // 2:1 (Specialist)
            default: return TradeManager.Instance.GetBestTradeCost(player, giveType); // 0: Auto
        }
    }

    public void RefreshTradeUI()
    {
        if (PlayerManager.Instance == null || TradeManager.Instance == null) return;

        Player currentPlayer = PlayerManager.Instance.currentPlayer;
        ResourceType giveType = resourceOptions[giveDropdown.value];
        ResourceType receiveType = resourceOptions[receiveDropdown.value];

        int selectedCost = GetSelectedRate(currentPlayer, giveType);
        int owned = currentPlayer.GetResourceCount(giveType);
        bool hasRight = TradeManager.Instance.IsRateAvailable(currentPlayer, giveType, selectedCost);

        if (rateInfoText != null)
        {
            if (!hasRight)
            {
                rateInfoText.text = $"Rate: {selectedCost} -> 1 (No Harbor!)\nOwned: {owned}";
            }
            else
            {
                rateInfoText.text = $"Rate: {selectedCost} -> 1\nOwned: {owned}";
            }
        }

        if (confirmTradeButton != null)
        {
            confirmTradeButton.interactable = TradeManager.Instance.CanTradeWithBank(currentPlayer, giveType, receiveType, selectedCost);
        }
    }

    private void OnConfirmTradeClicked()
    {
        Player currentPlayer = PlayerManager.Instance.currentPlayer;
        ResourceType giveType = resourceOptions[giveDropdown.value];
        ResourceType receiveType = resourceOptions[receiveDropdown.value];
        int selectedCost = GetSelectedRate(currentPlayer, giveType);

        if (TradeManager.Instance.ExecuteBankTrade(currentPlayer, giveType, receiveType, selectedCost))
        {
            RefreshTradeUI();
        }
    }
}
