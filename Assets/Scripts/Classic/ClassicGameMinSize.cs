using UnityEngine;

public static class ClassicGameMinSize
{
    public const int MinWidth = 5;
    public const int MaxWidth = 100;
    public const int MinHeight = 5;
    public const int MaxHeight = 100;    

    public static int MaxMines(int width, int height)
    {
        return Mathf.Max(1, width * height / 2); 
    }

    public static int MinMines(int width, int height)
    {
        return Mathf.Max(1, width * height / 10);
    }

    public static int ClampValue(int value, int min, int max)
    {
        return Mathf.Clamp(value, min, max);
    }
}
