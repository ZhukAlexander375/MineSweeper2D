using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Sector _sectorPrefab;
    [SerializeField] private int _sectorSize = 8;

    public bool IsFirstClick;


    private Camera mainCamera;
    private Grid _grid;
    private Vector2 _cellGap;
    private int initialSectorsVisibleInRange = 2;

    private Dictionary<Vector2Int, Sector> _sectors = new Dictionary<Vector2Int, Sector>();
   

    void Start()
    {
        mainCamera = Camera.main;
        _grid = GetComponent<Grid>();

        _cellGap.x = _grid.cellGap.x;
        _cellGap.y = _grid.cellGap.y;

        GenerateInitialSectors();

    }

    private void Update()
    {
        UpdateVisibleChunks();

        if (Input.GetMouseButtonDown(0))
        {
            GetSectorAtClick(0);
            //Reveal();
        }

        if (Input.GetMouseButtonDown(1))
        {
            GetSectorAtClick(1);
            //Reveal();
        }
        if (Input.GetMouseButtonDown(2))
        {
            GetSectorAtClick(2);            
        }
    }

    public void OnFirstClick()
    {
        if (!IsFirstClick)
        {
            IsFirstClick = true;
        }
    }

    public void GenerateFirstMines(Sector startingSector, Cell startingCell, int minesCount)
    {
        if (IsFirstClick)
        {
            return;
        }

        IsFirstClick = true;

        List<Sector> _sectorsToActivate = GetAdjacentSectors(startingSector);
        _sectorsToActivate.Add(startingSector);

        foreach (var sector in _sectorsToActivate)
        {            
            if (!sector.IsActive)
            {                
                GenerateMinesInSector(sector, startingCell, minesCount);
                GenerateNumbersInSector(sector);
                sector.IsActive = true;
            }
        }
    }

    private void GenerateMinesInSector(Sector sector, Cell startingCell, int minesCount)
    {
        int generatedMines = 0;
        while (generatedMines < minesCount)
        {
            Vector3Int randomPosition = new Vector3Int(
                Random.Range(0, _sectorSize),
                Random.Range(0, _sectorSize),
                0
            );

            Cell cell = sector.GetCell(randomPosition);

            if (cell.CellState != CellState.Mine && !IsAdjacent(startingCell, cell))
            {
                cell.CellState = CellState.Mine;
                generatedMines++;
            }
        }
    }

    private void GenerateNumbersInSector(Sector sector)
    {
        foreach (var cell in sector.Cells)
        {
            if (cell.Value.CellState != CellState.Mine)
            {
                int adjacentMines = CountAdjacentMines(cell.Value, sector);
                cell.Value.CellNumber = adjacentMines;

                if (adjacentMines > 0)
                {
                    cell.Value.CellState = CellState.Number;
                }
                else
                {
                    cell.Value.CellState = CellState.Empty;
                }
            }
        }
    }

    private int CountAdjacentMines(Cell cell, Sector sector)
    {
        int count = 0;
        Vector3Int cellPos = cell.CellPosition;

        // Проверка мин в текущем секторе
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                Vector3Int adjacentPos = new Vector3Int(cellPos.x + dx, cellPos.y + dy, 0);
                if (sector.GetCell(adjacentPos)?.CellState == CellState.Mine)
                    count++;
            }
        }

        // Проверка мин в соседних секторах только на границах
        List<Sector> adjacentSectors = GetAdjacentSectors(sector);
        foreach (var adjacentSector in adjacentSectors)
        {
            foreach (var offset in GetBorderOffsets(cellPos))
            {
                Vector3Int adjacentPos = cellPos + offset;
                if (adjacentSector.GetCell(adjacentPos)?.CellState == CellState.Mine)
                    count++;
            }
        }

        return count;
    }

    //позиции границ для соседних секторов
    private List<Vector3Int> GetBorderOffsets(Vector3Int cellPos)
    {
        // Смещения для поиска соседних ячеек на границах секторов
        return new List<Vector3Int>
        {
            new Vector3Int(-1, 0, 0), new Vector3Int(1, 0, 0),
            new Vector3Int(0, -1, 0), new Vector3Int(0, 1, 0),
            new Vector3Int(-1, -1, 0), new Vector3Int(1, 1, 0),
            new Vector3Int(-1, 1, 0), new Vector3Int(1, -1, 0)
        };
    }

    private List<Sector> GetAdjacentSectors(Sector currentSector)
    {
        Vector2Int sectorPosition = _sectors.FirstOrDefault(x => x.Value == currentSector).Key;
        List<Sector> adjacentSectors = new List<Sector>();
        for (int offsetX = -1; offsetX <= 1; offsetX++)
        {
            for (int offsetY = -1; offsetY <= 1; offsetY++)
            {
                Vector2Int adjacentPos = sectorPosition + new Vector2Int(offsetX, offsetY);
                
                if (_sectors.TryGetValue(adjacentPos, out Sector adjacentSector))
                {
                    adjacentSectors.Add(adjacentSector);
                }
            }
        }

        return adjacentSectors;

    }

    private bool IsAdjacent(Cell startingCell, Cell cell)
    {
        Vector3Int start = startingCell.CellPosition;
        Vector3Int pos = cell.CellPosition;

        return Mathf.Abs(start.x - pos.x) <= 1 && Mathf.Abs(start.y - pos.y) <= 1;
    }

    private void GetSectorAtClick(int click)
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 0;

        Vector2Int sectorPosition = new Vector2Int(
            Mathf.FloorToInt(worldPosition.x / _sectorSize),
            Mathf.FloorToInt(worldPosition.y / _sectorSize)
        );

        if (_sectors.TryGetValue(sectorPosition, out Sector clickedSector))
        {
            Vector2 localPositionInChunk = worldPosition - new Vector3(sectorPosition.x * _sectorSize, sectorPosition.y * _sectorSize, 0);
            int cellX = Mathf.FloorToInt(localPositionInChunk.x);
            int cellY = Mathf.FloorToInt(localPositionInChunk.y);
            
            clickedSector.HandleCellClick(cellX, cellY, click);
        }
    }


    [ContextMenu("Create")]
    private void GenerateInitialSectors()
    {
        for (int x = -initialSectorsVisibleInRange; x <= initialSectorsVisibleInRange; x++)
        {
            for (int y = -initialSectorsVisibleInRange; y <= initialSectorsVisibleInRange; y++)
            {
                var sectorPosition = new Vector2Int(x, y);
                CreateSector(sectorPosition);
            }
        }
    }

    private void CreateSector(Vector2Int position)
    {
        if (!_sectors.ContainsKey(position))
        {
            var sectorWorldPosition = new Vector3(position.x * _sectorSize, position.y * _sectorSize, 0);
            var newSector = Instantiate(_sectorPrefab, sectorWorldPosition, Quaternion.identity, transform);
            newSector.SetManager(this);
            _sectors.Add(position, newSector);
        }
    }

    private void UpdateVisibleChunks()
    {
        var currentChunkPosition = GetCurrentChunkPosition();
        var viewDistance = Mathf.CeilToInt((mainCamera.orthographicSize) / _sectorSize);

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                var chunkPosition = new Vector2Int(currentChunkPosition.x + x, currentChunkPosition.y + y);
                CreateSector(chunkPosition);
            }
        }
    }

    private Vector2Int GetCurrentChunkPosition()
    {
        var x = mainCamera.transform.position.x / _sectorSize;
        var y = mainCamera.transform.position.y / _sectorSize;
        return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
    } 
}
