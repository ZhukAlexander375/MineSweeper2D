using UnityEngine;

public class CellGrid
{
    private readonly BaseCell[,] cells;

    public int Width => cells.GetLength(0);
    public int Height => cells.GetLength(1);    

    public BaseCell this[int x, int y] => cells[x, y];

    public CellGrid(int width, int height)
    {
        cells = new BaseCell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y] = new BaseCell
                {
                    CellPosition = new Vector3Int(x, y, 0),
                    CellState = CellState.Empty
                };
            }
        }
    }

    public void GenerateMines(BaseCell startingCell, int amount)
    {
        int width = Width;
        int height = Height;

        for (int i = 0; i < amount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            BaseCell cell = cells[x, y];

            while (cell.CellState == CellState.Mine || IsAdjacent(startingCell, cell))
            {
                x++;

                if (x >= width)
                {
                    x = 0;
                    y++;

                    if (y >= height)
                    {
                        y = 0;
                    }
                }

                cell = cells[x, y];
            }

            cell.CellState = CellState.Mine;
        }
    }

    public void GenerateNumbers()
    {
        int width = Width;
        int height = Height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                BaseCell cell = cells[x, y];

                if (cell.CellState == CellState.Mine)
                {
                    continue;
                }

                cell.CellNumber = CountAdjacentMines(cell);
                cell.CellState = cell.CellNumber > 0 ? CellState.Number : CellState.Empty;
            }
        }
    }

    public int CountAdjacentMines(BaseCell cell)
    {
        int count = 0;

        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0)
                {
                    continue;
                }

                int x = cell.CellPosition.x + adjacentX;
                int y = cell.CellPosition.y + adjacentY;

                if (TryGetCell(x, y, out BaseCell adjacent) && adjacent.CellState == CellState.Mine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public int CountAdjacentFlags(BaseCell cell)
    {
        int count = 0;

        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0)
                {
                    continue;
                }

                int x = cell.CellPosition.x + adjacentX;
                int y = cell.CellPosition.y + adjacentY;

                if (TryGetCell(x, y, out BaseCell adjacent) && !adjacent.IsRevealed && adjacent.IsFlagged)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public BaseCell GetCell(int x, int y)
    {
        if (InBounds(x, y))
        {
            return cells[x, y];
        }
        else
        {
            return null;
        }
    }

    public bool TryGetCell(int x, int y, out BaseCell cell)
    {
        cell = GetCell(x, y);
        return cell != null;
    }

    public bool InBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public bool IsAdjacent(BaseCell a, BaseCell b)
    {
        if (a == null || b == null) return false;

        return Mathf.Abs(a.CellPosition.x - b.CellPosition.x) <= 1 &&
               Mathf.Abs(a.CellPosition.y - b.CellPosition.y) <= 1;
    }
}
