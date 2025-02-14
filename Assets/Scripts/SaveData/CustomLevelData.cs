[System.Serializable]
public class CustomLevelData
{
    public int Width;
    public int Height;
    public int MineCount;

    public CustomLevelData(LevelConfig level)
    {
        Width = level.Width;
        Height = level.Height;
        MineCount = level.MineCount;
    }
}
