using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public List<Player> players = new List<Player>();
    public int currentIndex = 0;

    public Player currentPlayer => players[currentIndex];

    public static PlayerManager EnsureInstance()
    {
        if(Instance != null)
        {
            return Instance;
        }

        PlayerManager existingManager = FindFirstObjectByType<PlayerManager>();

        if(existingManager != null)
        {
            Instance = existingManager;
            Instance.InitializePlayers();
            return Instance;
        }

        GameObject obj = new GameObject("PlayerManager");
        Instance = obj.AddComponent<PlayerManager>();
        return Instance;
    }

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializePlayers();
    }

    void InitializePlayers()
    {
        if(players.Count > 0)
        {
            return;
        }

        players.Add(new Player { name = "Red", color = PlayerColor.Red });
        players.Add(new Player { name = "Blue", color = PlayerColor.Blue });
        players.Add(new Player { name = "Green", color = PlayerColor.Orange });
        players.Add(new Player { name = "Yellow", color = PlayerColor.White });
    }

    public void NextPlayer()
    {
        currentIndex = (currentIndex + 1) % players.Count;
    }
}
