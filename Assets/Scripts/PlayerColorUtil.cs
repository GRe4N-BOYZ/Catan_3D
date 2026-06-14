using UnityEngine;

public static class PlayerColorUtil
{
    public static Color ToUnityColor(PlayerColor color)
    {
        switch (color)
        {
            case PlayerColor.Red:
                return Color.red;
            case PlayerColor.Blue:
                return Color.blue;
            case PlayerColor.Orange:
                return new Color(1f, 0.5f, 0f);
            case PlayerColor.White:
                return Color.white;
            default:
                return Color.gray;
        }
    }
}