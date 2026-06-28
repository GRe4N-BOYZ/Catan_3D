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
        NormalTurn
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
        if (!PlayerManager.Instance.setupPhase &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            DiceManager.Instance.RollDice();
        }
    }
}
