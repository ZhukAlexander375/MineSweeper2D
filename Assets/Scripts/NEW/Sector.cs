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
    public Dictionary<Vector3Int, Cell> Cells => _cells;

    private int _mineCount;

    private GridManager _gridManager;
    private Dictionary<Vector3Int, Cell> _cells = new Dictionary<Vector3Int, Cell>();


    private void Awake()
    {
        Tilemap = GetComponent<Tilemap>();
        InitializeSector();
        InitializeDict();

    }

    private void InitializeSector()
    {
        _mineCount = _sectorConfig.MineCount;
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
                    cell.CellPosition = position;
                    cell.CellState = CellState.Empty;
                }
            }
        }
    }

    public void HandleCellClick(int cellX, int cellY, int click)
    {        
        cellX %= 8;
        cellY %= 8;        

        if (cellX >= 0 && cellX < 8 && cellY >= 0 && cellY < 8) ///8 - hard
        {
            var clickedCell = _cells[new Vector3Int(cellX, cellY, 0)];

            if (!_gridManager.IsFirstClick)
            {
                _gridManager.GenerateFirstMines(this, clickedCell, _mineCount);
                                
                //IsActive = true;
            }

            else if (IsActive)
            {

            }

            Reveal(clickedCell);
            
            switch (click)
            {
                case 0:
                    Tilemap.SetTile(clickedCell.CellPosition, _tileConfig.TileFlag);
                    break;
                case 1:
                    Tilemap.SetTile(clickedCell.CellPosition, _tileConfig.TileMine);
                    break;
                case 2:
                    Tilemap.SetTile(clickedCell.CellPosition, _tileConfig.TileActive);
                    break;
            }
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

