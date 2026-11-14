using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator
{
    // 既存のHexBoardからリソース生成の定義を預かる
    private List<ResourceType> CreateResources()
    {
        List<ResourceType> resources = new List<ResourceType> { ResourceType.Desert };
        for (int i = 0; i < 4; i++) resources.Add(ResourceType.Wood);
        for (int i = 0; i < 4; i++) resources.Add(ResourceType.Sheep);
        for (int i = 0; i < 4; i++) resources.Add(ResourceType.Wheat);
        for (int i = 0; i < 3; i++) resources.Add(ResourceType.Brick);
        for (int i = 0; i < 3; i++) resources.Add(ResourceType.Ore);
        return resources;
    }

    private List<int> CreateNumbers()
    {
        return new List<int> { 2, 3, 3, 4, 4, 5, 5, 6, 6, 8, 8, 9, 9, 10, 10, 11, 11, 12 };
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            T tmp = list[i];
            list[i] = list[rnd];
            list[rnd] = tmp;
        }
    }

    public BoardData GenerateBoard(BoardDifficulty difficulty)
    {
        // 【追加】今どの難易度で生成が始まったかをConsoleに出力する
        Debug.Log($"★盤面生成モード：{difficulty} で作成しました！");
        
        switch (difficulty)
        {
            case BoardDifficulty.Standard:
                return GenerateStandardBoard();

            case BoardDifficulty.Challenging:
                return GenerateChallengingBoard();

            case BoardDifficulty.Random:
                return GenerateRandomBoard();

            default:
                return GenerateStandardBoard();
        }
    }

    public BoardData GenerateRandomBoard()
    {
        List<ResourceType> resources = CreateResources();
        List<int> numbers = CreateNumbers();

        ShuffleList(resources);
        ShuffleList(numbers);

        BoardData boardData = new BoardData();
        int numIdx = 0;

        for (int i = 0; i < resources.Count; i++)
        {
            TileData tile = new TileData();
            tile.resourceType = resources[i];
            tile.numberToken = (tile.resourceType == ResourceType.Desert) ? 0 : numbers[numIdx++];
            boardData.tiles.Add(tile);
        }

        return boardData;
    }

    // ★最重要メソッド：公式のバランスルールに沿ったBoardDataを生成して返す
    public BoardData GenerateStandardBoard()
    {
        List<ResourceType> baseResources = CreateResources();
        List<int> baseNumbers = CreateNumbers();
        int[] rows = { 3, 4, 5, 4, 3 };

        List<ResourceType> finalResources = null;
        List<int> finalNumbers = null;
        bool isBalancedLayout = false;
        int safetyCount = 0;

        // 条件を満たす配置ができるまでシャッフルを繰り返す（リジェクトサンプリング）
        while (!isBalancedLayout && safetyCount < 1000)
        {
            safetyCount++;
            List<ResourceType> shuffledRes = new List<ResourceType>(baseResources);
            List<int> shuffledNum = new List<int>(baseNumbers);
            ShuffleList(shuffledRes);
            ShuffleList(shuffledNum);

            if (EvaluateBoardBalance(shuffledRes, shuffledNum, rows))
            {
                finalResources = shuffledRes;
                finalNumbers = shuffledNum;
                isBalancedLayout = true;
            }
        }

        // ループを抜けたら、確定した配置リストから「BoardData」を組み立てる
        BoardData boardData = new BoardData();
        int numIdx = 0;
        int resIdx = 0;

        for (int i = 0; i < baseResources.Count; i++)
        {
            TileData tile = new TileData();
            tile.resourceType = finalResources[resIdx++];
            
            if (tile.resourceType == ResourceType.Desert)
                tile.numberToken = 0;
            else
                tile.numberToken = finalNumbers[numIdx++];

            boardData.tiles.Add(tile);
        }

        return boardData;
    }

        // =========================================================
    // ▼ Challengingモード専用：激戦区・偏り生成アルゴリズム ▼
    // =========================================================
    public BoardData GenerateChallengingBoard()
    {
        List<ResourceType> baseResources = CreateResources();
        List<int> baseNumbers = CreateNumbers();
        int[] rows = { 3, 4, 5, 4, 3 };

        List<ResourceType> bestResources = null;
        List<int> bestNumbers = null;
        int highestBiasScore = -1;

        // 200パターンの候補を生成し、最も「偏りスコア」が高いものを採用する（トーナメント選抜方式）
        for (int attempt = 0; attempt < 200; attempt++)
        {
            List<ResourceType> shuffledRes = new List<ResourceType>(baseResources);
            List<int> shuffledNum = new List<int>(baseNumbers);
            ShuffleList(shuffledRes);
            ShuffleList(shuffledNum);

            int score = CalculateChallengingScore(shuffledRes, shuffledNum, rows);

            if (score > highestBiasScore)
            {
                highestBiasScore = score;
                bestResources = shuffledRes;
                bestNumbers = shuffledNum;
            }
        }

        Debug.Log($"🔥 Challenging盤面確定！ 偏りスコア: {highestBiasScore}点");

        // 最もスコアが高かった配置を BoardData に変換して返す
        BoardData boardData = new BoardData();
        int numIdx = 0;
        int resIdx = 0;

        for (int i = 0; i < baseResources.Count; i++)
        {
            TileData tile = new TileData();
            tile.resourceType = bestResources[resIdx++];
            tile.numberToken = (tile.resourceType == ResourceType.Desert) ? 0 : bestNumbers[numIdx++];
            boardData.tiles.Add(tile);
        }

        return boardData;
    }

    // 【Evaluator】盤面の「偏り・激戦区度合い」をスコア化する評価関数
    private int CalculateChallengingScore(List<ResourceType> res, List<int> nums, int[] rows)
    {
        int tileTotal = res.Count;
        int[] tileNumbers = new int[tileTotal];
        int numIdx = 0;

        for (int i = 0; i < tileTotal; i++)
        {
            tileNumbers[i] = (res[i] == ResourceType.Desert) ? 0 : nums[numIdx++];
        }

        int[] rowStarts = { 0, 3, 7, 12, 16 };
        int currentTileIdx = 0;
        int biasScore = 0;
        bool hasRedAdjacency = false; // 6と8の隣接が1つでもあるか

        for (int row = 0; row < rows.Length; row++)
        {
            for (int col = 0; col < rows[row]; col++)
            {
                int myNum = tileNumbers[currentTileIdx];
                ResourceType myRes = res[currentTileIdx];

                // 隣接するタイルのインデックスリストを取得
                List<int> neighbors = new List<int>();

                // 右隣
                if (col + 1 < rows[row]) neighbors.Add(currentTileIdx + 1);

                // 下の行との隣接
                if (row + 1 < rows.Length)
                {
                    int nextRowStart = rowStarts[row + 1];
                    if (row < 2)
                    {
                        neighbors.Add(nextRowStart + col);
                        neighbors.Add(nextRowStart + col + 1);
                    }
                    else
                    {
                        if (col > 0) neighbors.Add(nextRowStart + col - 1);
                        if (col < rows[row + 1]) neighbors.Add(nextRowStart + col);
                    }
                }

                // 隣接タイルと比較してスコアを加算
                foreach (int neighborIdx in neighbors)
                {
                    int neighborNum = tileNumbers[neighborIdx];
                    ResourceType neighborRes = res[neighborIdx];

                    // 1. 赤数字（6, 8）同士が隣接していたら超特大ボーナス！
                    if ((myNum == 6 || myNum == 8) && (neighborNum == 6 || neighborNum == 8))
                    {
                        biasScore += 15;
                        hasRedAdjacency = true;
                    }
                    // 2. 赤数字の隣に準高確率（5, 9）が来てもボーナス（超激戦区の形成）
                    else if (((myNum == 6 || myNum == 8) && (neighborNum == 5 || neighborNum == 9)) ||
                             ((myNum == 5 || myNum == 9) && (neighborNum == 6 || neighborNum == 8)))
                    {
                        biasScore += 5;
                    }

                    // 3. 同じ資源が隣接していたらボーナス（資源の偏り・独占の誘発）
                    if (myRes != ResourceType.Desert && myRes == neighborRes)
                    {
                        biasScore += 3;
                    }
                }

                currentTileIdx++;
            }
        }

        // もし赤数字の隣接が1つもなければ、Challengingとしては失格（スコア0）にする
        if (!hasRedAdjacency) return 0;

        return biasScore;
    }


    // 【Evaluator】6と8が隣接していないかをシミュレーション判定する
    private bool EvaluateBoardBalance(List<ResourceType> res, List<int> nums, int[] rows)
    {
        int tileTotal = res.Count;
        int[] tileNumbers = new int[tileTotal];
        int numIdx = 0;

        for (int i = 0; i < tileTotal; i++)
        {
            if (res[i] == ResourceType.Desert) tileNumbers[i] = 0;
            else tileNumbers[i] = nums[numIdx++];
        }

        int[] rowStarts = { 0, 3, 7, 12, 16 };
        int currentTileIdx = 0;

        for (int row = 0; row < rows.Length; row++)
        {
            for (int col = 0; col < rows[row]; col++)
            {
                int myNum = tileNumbers[currentTileIdx];
                if (myNum == 6 || myNum == 8)
                {
                    if (col + 1 < rows[row])
                    {
                        if (tileNumbers[currentTileIdx + 1] == 6 || tileNumbers[currentTileIdx + 1] == 8) return false;
                    }
                    if (row + 1 < rows.Length)
                    {
                        int nextRowStart = rowStarts[row + 1];
                        if (row < 2)
                        {
                            if (tileNumbers[nextRowStart + col] == 6 || tileNumbers[nextRowStart + col] == 8) return false;
                            if (tileNumbers[nextRowStart + col + 1] == 6 || tileNumbers[nextRowStart + col + 1] == 8) return false;
                        }
                        else
                        {
                            if (col > 0 && (tileNumbers[nextRowStart + col - 1] == 6 || tileNumbers[nextRowStart + col - 1] == 8)) return false;
                            if (col < rows[row + 1] && (tileNumbers[nextRowStart + col] == 6 || tileNumbers[nextRowStart + col] == 8)) return false;
                        }
                    }
                }
                currentTileIdx++;
            }
        }
        return true;
    }
}
