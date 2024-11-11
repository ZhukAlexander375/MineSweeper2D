using UnityEngine;
using UnityEngine.Tilemaps;

public class Sector
{
    public Vector2Int Position { get; private set; }
    public CellGrid CellGrid { get; private set; }

    public Sector(Vector2Int position, int width, int height, int mineCount)
    {
        Position = position;
        CellGrid = new CellGrid(width, height);
        CellGrid.GenerateMines(mineCount);
    }

    public void InitializeMines(int mineCount)
    {

    }
}
