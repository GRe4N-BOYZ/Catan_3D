using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public List<Player> players = new List<Player>();
    public int currentIndex = 0;
    public bool setupPhase = true;
    public Player currentPlayer => players[currentIndex];
     public int initialPlacementCount = 0;
     public bool reverseOrder = false;
     public bool isSetupPhase = true;

     // 現在の手番のプレイヤーが、初期配置のセット（建物1＋道路1）を完了したか判定するメソッド
    public bool CanEndTurnDuringSetup()
    {
        // 通常ターンであれば、いつでもEndTurnしてOK
        if (!isSetupPhase) return true;

        // 【チェックロジック】
        // 初期配置フェーズの場合、GameManagerの現在のステート（GameState）が
        // 「配置アクションがすべて終わってEndTurnを待っている状態」に遷移しているか、
        // あるいはPlayerがこのターンに建てた数などのデータをもとに判定します。
        // ここでは一番確実な「GameManagerのステート監視」を前提にします。
        
        return true; // 既存のGameStateの変数に合わせて調整
    }

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

    public void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializePlayers();
    }

    public void InitializePlayers()
    {
        if(players.Count > 0)
        {
            return;
        }

        players.Add(new Player { name = "Red", color = PlayerColor.Red });
        players.Add(new Player { name = "Blue", color = PlayerColor.Blue });
        players.Add(new Player { name = "Orange", color = PlayerColor.Orange });
        players.Add(new Player { name = "White", color = PlayerColor.White });
    }

    public void NextPlayer()
    {
        if(!setupPhase)
        {
            //通常ターン
            currentIndex = (currentIndex + 1) % players.Count;
        } else
        {
            if(!reverseOrder)
            {
                //初期配置1週目
                currentIndex = (currentIndex + 1) % players.Count;
            } else
            {
                 // 初期配置2周目
                currentIndex--;

                if (currentIndex < 0)
                {
                    currentIndex = players.Count - 1;
                }
            }
        }
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateAll();
        }
    }

    public void FinishInitialRoad()
    {
        initialPlacementCount++;

        if (initialPlacementCount == 4)
        {
            reverseOrder = true;
            GameManager.Instance.ChangeState(GameManager.GameState.InitialSettlement);
            Debug.Log("初期配置2週目開始");
            return;
        }
        else if (initialPlacementCount == 8)
        {
            setupPhase = false;
            reverseOrder = false;

            StartTurn();

            Debug.Log("初期配置終了");
            
            return;
        }

        GameManager.Instance.ChangeState
        (
            GameManager.GameState.InitialSettlement
        );
        NextPlayer();
    }

    public void StartTurn()
    {
        GameManager.Instance.ChangeState
        (
            GameManager.GameState.RollDice
        );
        GameManager.Instance.ChangeBuildMode
        (
            BuildMode.None
        );
        UIManager.Instance.UpdateAll();
        Debug.Log($"{currentPlayer.name} Turn Start");
    }

    public void EndTurn()
    {
        NextPlayer();
        StartTurn();
    }
}
