using System.Collections.Generic;
using UnityEngine;

// 盤面上の1枚のタイルの設定データだけを保持する構造体
public struct TileData
{
    public ResourceType resourceType;
    public int numberToken;
}

// 盤面全体の配置データを保持するデータクラス
public class BoardData
{
    // タイルのデータを順番に格納するリスト
    public List<TileData> tiles = new List<TileData>();
}
