using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeUIManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("貿易ウィンドウ全体のパネル")]
    public GameObject tradePanel;

    [Tooltip("支払う資源を選ぶドロップダウン")]
    public TMP_Dropdown giveDropdown;

    [Tooltip("獲得する資源を選ぶドロップダウン")]
    public TMP_Dropdown receiveDropdown;

    [Tooltip("必要枚数などを表示するテキスト（例: 4枚 ➔ 1枚）")]
    public TMP_Text rateInfoText;

    [Tooltip("交換実行ボタン")]
    public Button confirmTradeButton;

    // ドロップダウンの選択肢（0:Wood, 1:Brick, 2:Sheep, 3:Wheat, 4:Ore）に対応する配列
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
        // 最初は貿易ウィンドウを閉じておく
        if (tradePanel != null) tradePanel.SetActive(false);

        // ドロップダウンの選択が変わったときに画面表示とボタンの押下可否を更新する
        if (giveDropdown != null) giveDropdown.onValueChanged.AddListener(_ => RefreshTradeUI());
        if (receiveDropdown != null) receiveDropdown.onValueChanged.AddListener(_ => RefreshTradeUI());
        if (confirmTradeButton != null) confirmTradeButton.onClick.AddListener(OnConfirmTradeClicked);
    }

    // 画面右下の「Trade」ボタンを押したときに呼ぶ（ウィンドウの開閉）
    public void ToggleTradePanel()
    {
        if (tradePanel == null) return;

        // PlayerAction中だけ開けるようにする
        if (!tradePanel.activeSelf && GameManager.Instance.currentState != GameManager.GameState.PlayerAction)
        {
            Debug.LogWarning("通常行動フェーズ中のみ貿易が可能です。");
            return;
        }

        tradePanel.SetActive(!tradePanel.activeSelf);

        if (tradePanel.activeSelf)
        {
            RefreshTradeUI();
        }
    }

    // 現在の選択状態に応じて、レート表示や「交換する」ボタンの有効/無効を更新
    public void RefreshTradeUI()
    {
        if (PlayerManager.Instance == null || TradeManager.Instance == null) return;

        Player currentPlayer = PlayerManager.Instance.currentPlayer;
        ResourceType giveType = resourceOptions[giveDropdown.value];
        ResourceType receiveType = resourceOptions[receiveDropdown.value];

        int cost = TradeManager.Instance.GetTradeCost(currentPlayer, giveType);
        int owned = currentPlayer.GetResourceCount(giveType);

        if (rateInfoText != null)
        {
            rateInfoText.text = $"Rate: {cost} -> 1\n(Owned: {owned})";
        }


        // 交換可能なときだけ確定ボタンを押せるようにする
        if (confirmTradeButton != null)
        {
            confirmTradeButton.interactable = TradeManager.Instance.CanTradeWithBank(currentPlayer, giveType, receiveType);
        }
    }

    private void OnConfirmTradeClicked()
    {
        Player currentPlayer = PlayerManager.Instance.currentPlayer;
        ResourceType giveType = resourceOptions[giveDropdown.value];
        ResourceType receiveType = resourceOptions[receiveDropdown.value];

        if (TradeManager.Instance.ExecuteBankTrade(currentPlayer, giveType, receiveType))
        {
            // 交換に成功したら所持数表示などを最新化
            RefreshTradeUI();
        }
    }
}
