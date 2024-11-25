using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Sector : MonoBehaviour
{
    [SerializeField] private List<TileSetConfig> _tileSets;
    //[SerializeField] private LevelConfig _sectorConfig;
    
    public Tilemap Tilemap { get; private set; }
    public bool IsActive;
    public bool IsFirstCellActivated {  get; set; }
    public bool IsExplode { get; set; }
    public Dictionary<Vector3Int, InfiniteCell> Cells => _cells;   
       

    private InfiniteGridManager _infiniteGridManager;
    private Dictionary<Vector3Int, InfiniteCell> _cells = new Dictionary<Vector3Int, InfiniteCell>();
    private TileSetConfig _currentTileSet;
    private int _currentTileSetIndex;


    private void Start()
    {
        Tilemap = GetComponent<Tilemap>();
        //InitializeSector();
        InitializeDict();
        SignalBus.Subscribe<OnCellActiveSignal>(SectorActivate);
        SignalBus.Subscribe<ThemeChangeSignal>(OnThemeChanged);
        TryApplyTheme(ThemeManager.Instance.CurrentThemeIndex);
    }

    private void InitializeSector()
    {
        //_mineCount = _sectorConfig.MineCount;       //random mines (2..10000)
        //_mineCount = Random.Range(5, 15);
    }

    private void InitializeDict()
    {
        Vector3Int boundsMin = Tilemap.cellBounds.min;
        Vector3Int boundsMax = Tilemap.cellBounds.max;

        for (int x = boundsMin.x; x < boundsMax.x; x++)
        {
            for (int y = boundsMin.y; y < boundsMax.y; y++)
            {
                Vector3Int position = new Vector3Int(x, y, 0);

                TileBase tile = Tilemap.GetTile(position);

                if (tile != null)
                {
                    InfiniteCell cell = new InfiniteCell();
                    _cells[new Vector3Int(x, y, 0)] = cell;
                    cell.CellPosition = position + new Vector3Int((int)transform.position.x, (int)transform.position.y, 0);
                    cell.CellState = CellState.Empty;
                    
                    cell.SetOwnerSector(this);
                    cell.SetGridManager(_infiniteGridManager);
                }
            }
        }
    }

    public void HandleCellClick(int cellX, int cellY)
    {

        cellX %= 8;
        cellY %= 8;

        if (cellX >= 0 && cellX < 8 && cellY >= 0 && cellY < 8) ///8 - hard
        {
            var clickedCell = _cells[new Vector3Int(cellX, cellY, 0)];

            if (!_infiniteGridManager.IsFirstClick)
            {
                _infiniteGridManager.GenerateFirstSectors(this, clickedCell);

                _infiniteGridManager.Reveal(this, clickedCell);

                clickedCell.IsActive = true; // pri samom pervom click 
            }

            else if (clickedCell.IsActive)
            {
                _infiniteGridManager.Reveal(this, clickedCell);
            }

            //_gridManager.Reveal(this, clickedCell);
        }
    }

    public void HandleCellFlag(int cellX, int cellY)
    {

        cellX %= 8;
        cellY %= 8;

        if (cellX >= 0 && cellX < 8 && cellY >= 0 && cellY < 8) ///8 - hard
        {
            var clickedCell = _cells[new Vector3Int(cellX, cellY, 0)];

            if (!_infiniteGridManager.IsFirstClick)
            {
                return;
            }

            else if (clickedCell.IsActive)
            {
                _infiniteGridManager.Flag(this, clickedCell);
            }
        }
    }

    public void HandleChord(int cellX, int cellY)
    {
        cellX %= 8;
        cellY %= 8;
        
        if (cellX >= 0 && cellX < 8 && cellY >= 0 && cellY < 8) // 8 - размер сектора
        {
            var clickedCell = _cells[new Vector3Int(cellX, cellY, 0)];

            if (!_infiniteGridManager.IsFirstClick)
            {
                return; // Игнорируем до первого клика
            }

            if (clickedCell.IsRevealed && clickedCell.CellState == CellState.Number)
            {                
                // Проверяем, правильно ли расставлены флаги вокруг ячейки
                int adjacentFlags = _infiniteGridManager.CountAdjacentFlags(clickedCell);
                if (adjacentFlags >= clickedCell.CellNumber)
                {                    
                    _infiniteGridManager.Unchord(this, clickedCell);
                }
            }
        }
    }

    public List<InfiniteCell> GetCellValues()
    {
        List<InfiniteCell> cells = new List<InfiniteCell>();

        foreach (var cell in _cells.Values)
        {
            cells.Add(cell);
        }

        return cells;
    }

    public void DrawSector()
    {
        int width = _infiniteGridManager.SectorSize;
        int height = _infiniteGridManager.SectorSize;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                InfiniteCell cell = _cells[new Vector3Int(x, y, 0)];
                Tilemap.SetTile(new Vector3Int(x, y, 0), GetTile(cell));
            }
        }
    }

    public void GenerateNumbersInSector()
    {
        _infiniteGridManager.GenerateNumbers(this);
    }

    private Tile GetTile(InfiniteCell cell)
    {
        if (cell.IsRevealed)
        {
            return GetRevealedTile(cell);
        }

        else if (cell.IsFlagged)
        {
            return _tileSets[_currentTileSetIndex].TileFlag;
        }

        else if (cell.IsActive && !cell.IsRevealed)
        {
            return _tileSets[_currentTileSetIndex].TileActive;            ///////////////
        }

        else
        {
            return _tileSets[_currentTileSetIndex].TileInactive;              /////////////////////
        }
    }

    private Tile GetRevealedTile(InfiniteCell cell)
    {
        switch (cell.CellState)
        {
            case CellState.Empty: return _tileSets[_currentTileSetIndex].TileEmpty;
            case CellState.Mine: return cell.IsExploded ? _tileSets[_currentTileSetIndex].TileMine : _tileSets[_currentTileSetIndex].TileExploded;
            case CellState.Number: return GetNumberTile(cell);
            default: return null;
        }
    }

    private Tile GetNumberTile(InfiniteCell cell)
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


    public InfiniteCell GetCell(Vector3Int coordinates)
    {
        if (_cells.ContainsKey(coordinates))
        {
            return _cells[coordinates];
        }
        return null;
    }

    public void SetManager(InfiniteGridManager gridManager)
    {
        _infiniteGridManager = gridManager;
    }    

    public void SectorActivate(OnCellActiveSignal signal)
    {
        if (signal.Cell.Sector == this)
        {
            var cell = signal.Cell;
            
            if (!IsFirstCellActivated)
            {
                
                _infiniteGridManager.GenerateSectors(this, cell);
                IsFirstCellActivated = true;
                IsActive = true;                
            }

            if (IsFirstCellActivated)
            {
                return;
            }
        }        
    }

    public void TryApplyTheme(int themeIndex)
    {
        if (themeIndex < 0 || themeIndex >= _tileSets.Count)
        {
            return;
        }

        _currentTileSet = _tileSets[themeIndex];
        _currentTileSetIndex = themeIndex;
        if (Tilemap != null)
        {
            RedrawGrid();
        }
    }

    public void RedrawGrid()
    {
        if (Tilemap == null) return;
               
        foreach (var cellPosition in _cells.Keys)
        {
            var cell = _cells[cellPosition];
            Tilemap.SetTile(cellPosition, GetTile(cell));
        }

        Tilemap.RefreshAllTiles();
    }

    private void OnThemeChanged(ThemeChangeSignal signal)
    {
        TryApplyTheme(signal.ThemeIndex);
    }

    private void OnDestroy()
    {
        SignalBus.Unsubscribe<OnCellActiveSignal>(SectorActivate);
        SignalBus.Unsubscribe<ThemeChangeSignal>(OnThemeChanged);
    }











    [ContextMenu("Test")]
    public void TestSector()
    {        
        StartCoroutine(OpenCellsGradually());
    }

    private IEnumerator OpenCellsGradually()
    {        
        foreach (var pos in _cells.Keys)
        {            
            Vector3Int tilePosition = new Vector3Int(pos.x, pos.y, 0);
            Tilemap.SetTile(tilePosition, _tileSets[_currentTileSetIndex].TileFlag);
            yield return new WaitForSeconds(0.5f);
        }        
    }
}

