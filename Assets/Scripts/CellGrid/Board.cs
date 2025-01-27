using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class Board : MonoBehaviour
{ 
    public Tilemap Tilemap { get; private set; }

    [SerializeField] private List<TileSetConfig> _tileSets;

    private TileSetConfig _currentTileSet;
    private int _currentTileSetIndex;


    private void Awake()
    {
        Tilemap = GetComponent<Tilemap>();        
    }

    private void Start()
    {
        SignalBus.Subscribe<ThemeChangeSignal>(OnThemeChanged);
        TryApplyTheme(ThemeManager.Instance.CurrentThemeIndex);
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
    public void ClearGrid()
    {
        if (Tilemap != null)
        {
            Tilemap.ClearAllTiles();
        }
    }

    public void Draw(CellGrid grid)
    {
        if (ThemeManager.Instance != null)
        {
            _currentTileSetIndex = ThemeManager.Instance.CurrentThemeIndex;
        }        

        // Î×ÈÙÅÍÈÅ!!!!!!!!!!!!!!!!!!!!!!!! ×ÅÊ ÏÐÈ ÑÎÕÐÀÍÅÍÈÈ ×ÒÎ ÄÅËÀÒÜ???????????
        ClearGrid();

        if (grid == null)
        {
            Debug.LogWarning("Grid is null. Cannot draw.");
            return;
        }

        int width = grid.Width;
        int height = grid.Height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                BaseCell cell = grid[x, y];
                Tilemap.SetTile(cell.GlobalCellPosition, GetTile(cell));
            }
        }
    }

    private Tile GetTile(BaseCell cell)
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

    private Tile GetRevealedTile(BaseCell cell)
    {
        switch (cell.CellState)
        {
            case CellState.Empty: return _tileSets[_currentTileSetIndex].TileEmpty;
            case CellState.Mine: return cell.IsExploded ? _tileSets[_currentTileSetIndex].TileExploded : _tileSets[_currentTileSetIndex].TileMine;
            case CellState.Number: return GetNumberTile(cell);
            default: return null;
        }
    }

    private Tile GetNumberTile(BaseCell cell)
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

    public void TryApplyTheme(int themeIndex)
    {
        if (themeIndex < 0 || themeIndex >= _tileSets.Count)
        {
            Debug.LogWarning($"Íåäîïóñòèìûé èíäåêñ òåìû: {themeIndex}");
            return;
        }
        
        _currentTileSet = _tileSets[themeIndex];
        _currentTileSetIndex = themeIndex;       
                
        RedrawGrid();
    }

    private void RedrawGrid()
    {
        if (Tilemap == null) return;
        {
            Tilemap.RefreshAllTiles();
        }
    }

    private void OnThemeChanged(ThemeChangeSignal signal)
    {
        TryApplyTheme(signal.ThemeIndex);
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<ThemeChangeSignal>(OnThemeChanged);
    }
}
