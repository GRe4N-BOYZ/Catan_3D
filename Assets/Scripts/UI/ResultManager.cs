using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject resultPanel;
    public TMP_Text winnerText;
    public Button titleButton;
    public Button retryButton;

    [Header("Scene Settings")]
    public string titleSceneName = "TitleScene";

    private void Start()
    {
        if (resultPanel != null) resultPanel.SetActive(false);

        if (titleButton != null) titleButton.onClick.AddListener(ReturnToTitle);
        if (retryButton != null) retryButton.onClick.AddListener(RetryGame);
    }

    // 勝者が決定したときに呼び出す
    public void ShowResult(Player winner)
    {
        if (resultPanel == null) return;

        resultPanel.SetActive(true);

        if (winnerText != null && winner != null)
        {
            winnerText.text = $"Winner: {winner.name}!";
        }
    }

    public void ReturnToTitle()
    {
        SceneManager.LoadScene(titleSceneName);
    }

    public void RetryGame()
    {
        // 現在のゲームシーンを再読み込みして同じ難易度で再戦する
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
