using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("ゲーム本編のシーン名を正確に入力してください")]
    public string gameSceneName = "MainScene";

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

    private void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
