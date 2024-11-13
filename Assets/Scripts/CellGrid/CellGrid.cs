using UnityEngine;

public class CellGrid
{
    private readonly Cell[,] cells;

    public int Width => cells.GetLength(0);
    public int Height => cells.GetLength(1);    

    public Cell this[int x, int y] => cells[x, y];

    public CellGrid(int width, int height)
    {
        cells = new Cell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y] = new Cell
                {
                    CellPosition = new Vector3Int(x, y, 0),
                    CellState = CellState.Empty
                };
            }
        }
    }

    public void GenerateMines(Cell startingCell, int amount)
    {
        int width = Width;
        int height = Height;

        for (int i = 0; i < amount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            Cell cell = cells[x, y];

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
                Cell cell = cells[x, y];

                if (cell.CellState == CellState.Mine)
                {
                    continue;
                }

                cell.CellNumber = CountAdjacentMines(cell);
                cell.CellState = cell.CellNumber > 0 ? CellState.Number : CellState.Empty;
            }
        }
    }

    public int CountAdjacentMines(Cell cell)
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

                if (TryGetCell(x, y, out Cell adjacent) && adjacent.CellState == CellState.Mine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public int CountAdjacentFlags(Cell cell)
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

                if (TryGetCell(x, y, out Cell adjacent) && !adjacent.IsRevealed && adjacent.IsFlagged)
                {
                    count++;
                }
            }
        }

        return count;
    }

    public Cell GetCell(int x, int y)
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

    public bool TryGetCell(int x, int y, out Cell cell)
    {
        cell = GetCell(x, y);
        return cell != null;
    }

    public bool InBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public bool IsAdjacent(Cell a, Cell b)
    {
        if (a == null || b == null) return false;

        return Mathf.Abs(a.CellPosition.x - b.CellPosition.x) <= 1 &&
               Mathf.Abs(a.CellPosition.y - b.CellPosition.y) <= 1;
    }
}
