using System.Collections.Generic;
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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentState = GameState.InitialSettlement;
    }
}
