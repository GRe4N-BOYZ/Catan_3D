using UnityEngine.InputSystem;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GameManager Instance;
    public enum GameState
    {
        InitialSettlement,
        InitialRoad,
        RollDice,
        MoveRobber,
        PlayerAction,
        GameEnd
    }

    public GameState currentState;
    public Vertex lastPlacedSettlement;
    public BuildMode currentBuildMode = BuildMode.None;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeState(GameState nextState)
    {
        currentState = nextState;
        Debug.Log($"State -> {currentState}");

        if(UIManager.Instance != null)
        {
            UIManager.Instance.UpdatePhaseUI();
        }
    }

    public void ChangeBuildMode(BuildMode mode)
    {
        currentBuildMode = mode;
        Debug.Log($"Build Mode : {mode}");

        if(UIManager.Instance != null)
        {
            UIManager.Instance.UpdateAll();
        }
    }

    /*private void DebugWin()
    {
        Debug.Log(
            $"DEBUG: {PlayerManager.Instance.currentPlayer.name} Wins!"
        );

        ChangeState(GameState.GameEnd);
    }*/
    
    private void Start()
    {
        currentState = GameState.InitialSettlement;
        ChangeBuildMode
        (
            BuildMode.Settlement
        );
    }

    private void Update()
    {
        if (currentState == GameState.RollDice &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            DiceManager.Instance.RollDice();
        }
    /*#if UNITY_EDITOR
        if(Keyboard.current.gKey.wasPressedThisFrame)
        {
            DebugWin();
        }
    #endif*/

        //rkey debug +10
        if (Keyboard.current.f1Key.wasPressedThisFrame)
        {
            Player player =
                PlayerManager.Instance.currentPlayer;

            player.DebugAddResources(10);

            UIManager.Instance.UpdateResourceUI();
        }
    }

    public void CheckGameEnd(Player player)
    {
        if(VictoryManager.Instance == null)
        {
            Debug.LogError("VictoryManagerが存在しません");
            return;
        }
        if(VictoryManager.Instance.CheckVictory(player))
        {
            Debug.Log($"{player.name} Wins!");
            ChangeState(GameState.GameEnd);
        }
    }
}
