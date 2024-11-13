using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class Board : MonoBehaviour
{
    public Tilemap Tilemap { get; private set; }
    
    [SerializeField] private List<TileSetConfig> _tileSets;
    private int _currentTileSetIndex = 0;


    private void Awake()
    {
        Tilemap = GetComponent<Tilemap>();
    }

    /*public void DrawFreeForm(FreeForm freeGrid, CellGrid grid)
    {
        var width = (int)Mathf.Sqrt(freeGrid.GridSize);

        for (int x = 0; x < freeGrid.GridSize; x++)
        {            
            if (freeGrid.ActiveCells[x])
            {
                Cell cell = grid[x % width, x / width];                   
                Tilemap.SetTile(cell.CellPosition, GetTile(cell));
            }

            *//*else
            {
                Cell cell = grid[x - 1, y-1];
                Tilemap.SetTile(cell.CellPosition, null);
            } *//*
            
        }
    }*/

    public void Draw(CellGrid grid)
    {
        int width = grid.Width;
        int height = grid.Height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = grid[x, y];
                Tilemap.SetTile(cell.CellPosition, GetTile(cell));
            }
        }
    }

    private Tile GetTile(Cell cell)
    {
        if (cell.IsRevealed)
        {
            return GetRevealedTile(cell);
        }
        else if (cell.IsFlagged)
        {
            return _tileSets[_currentTileSetIndex].TileFlag;
        }
        else
        {
            return _tileSets[_currentTileSetIndex].TileActive;
        }
    }

    private Tile GetRevealedTile(Cell cell)
    {
        switch (cell.CellState)
        {            
            case CellState.Empty: return _tileSets[_currentTileSetIndex].TileEmpty;
            case CellState.Mine: return cell.IsExploded ? _tileSets[_currentTileSetIndex].TileExploded : _tileSets[_currentTileSetIndex].TileMine;
            case CellState.Number: return GetNumberTile(cell);
            default: return null;
        }
    }

    private Tile GetNumberTile(Cell cell)
    {
        switch (cell.CellNumber)
        {
            case 1: return _tileSets[_currentTileSetIndex].TileNum1;
            case 2: return _tileSets[_currentTileSetIndex].TileNum2;
            case 3: return _tileSets[_currentTileSetIndex].TileNum3;
            case 4: return _tileSets[_currentTileSetIndex].TileNum4;
            case 5: return _tileSets[_currentTileSetIndex].TileNum5;
            case 6: return _tileSets[_currentTileSetIndex].TileNum6;
            case 7: return _tileSets[_currentTileSetIndex].TileNum7;
            case 8: return _tileSets[_currentTileSetIndex].TileNum8;
            default: return null;
        }
    }
}
