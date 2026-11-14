// 盤面生成の難易度を定義する列挙型
public enum BoardDifficulty
{
    Standard,     // 公式ルール準拠（バランス重視）
    Challenging,  // 偏り・格差重視
    Random        // 完全ランダム
}

// シーンをまたいで設定を保持する静的クラス
public static class GameSettings
{
    // デフォルトはStandardにしておく
    public static BoardDifficulty SelectedDifficulty = BoardDifficulty.Standard;
}