using UnityEngine;

public static class ClassicGameMinSize
{
    public const int MinWidth = 5;
    public const int MaxWidth = 50;
    public const int MinHeight = 5;
    public const int MaxHeight = 50;    

    public static int MaxMines(int width, int height)
    {
        return Mathf.RoundToInt(width * height * 0.35f); 
    }

    public static int MinMines(int width, int height)
    {
        return Mathf.RoundToInt(width * height * 0.2f); ;
    }

    public static int ClampValue(int value, int min, int max)
    {
        return Mathf.Clamp(value, min, max);
    }
}
