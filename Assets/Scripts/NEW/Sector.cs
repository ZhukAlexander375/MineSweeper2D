using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Sector : MonoBehaviour
{
    [SerializeField] private TileSetConfig _tileConfig;
    [SerializeField] private LevelConfig _sectorConfig;
    
    public Tilemap Tilemap { get; private set; }
    public bool IsActive { get; set; }
    public bool IsNumbered {  get; set; }
    public bool IsExplode { get; set; }
    public Dictionary<Vector3Int, Cell> Cells => _cells;    

    private int _mineCount;

    private GridManager _gridManager;
    private Dictionary<Vector3Int, Cell> _cells = new Dictionary<Vector3Int, Cell>();


    private void Start()
    {
        Tilemap = GetComponent<Tilemap>();
        InitializeSector();
        InitializeDict();

    }

    private void InitializeSector()
    {
        //_mineCount = _sectorConfig.MineCount;       //random mines (2..10000)
        _mineCount = Random.Range(8, 15);
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
                    Cell cell = new Cell();
                    _cells[new Vector3Int(x, y, 0)] = cell;
                    cell.CellPosition = position + new Vector3Int((int)transform.position.x, (int)transform.position.y, 0);
                    cell.CellState = CellState.Empty;
                    
                    cell.SetOwnerSector(this);
                    cell.SetGridManager(_gridManager);
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

            if (!_gridManager.IsFirstClick)
            {
                _gridManager.GenerateFirstMines(this, clickedCell, _mineCount);

                _gridManager.Reveal(this, clickedCell);

                clickedCell.IsActive = true; // pri samom pervom click 
            }

            else if (clickedCell.IsActive)
            {
                _gridManager.Reveal(this, clickedCell);
            }

            //_gridManager.Reveal(this, clickedCell);
        }
    }        

    public List<Cell> GetCellValues()
    {
        List<Cell> cells = new List<Cell>();

        foreach (var cell in _cells.Values)
        {
            cells.Add(cell);
        }

        return cells;
    }

    public void DrawSector()
    {
        int width = _gridManager.SectorSize;
        int height = _gridManager.SectorSize;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = _cells[new Vector3Int(x, y, 0)];
                Tilemap.SetTile(new Vector3Int(x, y, 0), GetTile(cell));
            }
        }
    }

    public void GenerateNumbersInSector()
    {
        _gridManager.GenerateNumbers(this);
    }

    private Tile GetTile(Cell cell)
    {
        if (cell.IsRevealed)
        {
            return GetRevealedTile(cell);
        }

        else if (cell.IsFlagged)
        {
            return _tileConfig.TileFlag;
        }

        else if (cell.IsActive && !cell.IsRevealed)
        {
            return _tileConfig.TileActive;
        }

        else
        {
            return _tileConfig.TileInactive;
        }
    }

    private Tile GetRevealedTile(Cell cell)
    {
        switch (cell.CellState)
        {
            case CellState.Empty: return _tileConfig.TileEmpty;
            case CellState.Mine: return cell.IsExploded ? _tileConfig.TileExploded : _tileConfig.TileMine;
            case CellState.Number: return GetNumberTile(cell);
            default: return null;
        }
    }

    private Tile GetNumberTile(Cell cell)
    {
        switch (cell.CellNumber)
        {
            case 1: return _tileConfig.TileNum1;
            case 2: return _tileConfig.TileNum2;
            case 3: return _tileConfig.TileNum3;
            case 4: return _tileConfig.TileNum4;
            case 5: return _tileConfig.TileNum5;
            case 6: return _tileConfig.TileNum6;
            case 7: return _tileConfig.TileNum7;
            case 8: return _tileConfig.TileNum8;
            default: return null;
        }
    }

    private void Reveal(Cell cell)
    {
        Debug.Log(cell.CellState);
    }     
    

    public Cell GetCell(Vector3Int coordinates)
    {
        if (_cells.ContainsKey(coordinates))
        {
            return _cells[coordinates];
        }
        return null;
    }

    public void SetManager(GridManager gridManager)
    {
        _gridManager = gridManager;
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
            Tilemap.SetTile(tilePosition, _tileConfig.TileFlag);
            yield return new WaitForSeconds(0.5f);
        }        
    }
}

