using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("ゲーム本編のシーン名を正確に入力してください")]
    public string gameSceneName = "MainScene";

    [Header("UI Panels")]
    [Tooltip("遊び方・操作説明のパネル")]
    public GameObject howToPlayPanel;

    private void Start()
    {
        // スタート時は説明パネルを閉じておく
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }
    }

    // Standardボタンから呼ぶ
    public void StartStandardGame()
    {
        GameSettings.SelectedDifficulty = BoardDifficulty.Standard;
        LoadGameScene();
    }

    // Challengingボタンから呼ぶ
    public void StartChallengingGame()
    {
        GameSettings.SelectedDifficulty = BoardDifficulty.Challenging;
        LoadGameScene();
    }

    // Randomボタンから呼ぶ
    public void StartRandomGame()
    {
        GameSettings.SelectedDifficulty = BoardDifficulty.Random;
        LoadGameScene();
    }

    // 「How to Play」ボタンと「Close」ボタンの両方から呼ぶ
    public void ToggleHowToPlayPanel()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(!howToPlayPanel.activeSelf);
        }
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
