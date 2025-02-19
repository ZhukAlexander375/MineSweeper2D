
using System.Collections.Generic;

[System.Serializable]

public class SimpleGridData
{
    public int Width;
    public int Height;
    public int MineCount;
    public List<CellData> Cells = new List<CellData>();
}
