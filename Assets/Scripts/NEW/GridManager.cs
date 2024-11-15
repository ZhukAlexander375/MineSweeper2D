using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    public int SectorSize => _sectorSize;

    [Header("Settings")]
    [SerializeField] private Sector _sectorPrefab;    
    [SerializeField] private int _minMinesCount;
    [SerializeField] private int _maxMinesCount;    

    public bool IsFirstClick;

    private int _sectorSize = 8;            ///
    private Camera mainCamera;
    private Grid _grid;
    private Vector2 _cellGap;
    private int initialSectorsVisibleInRange = 2;
    private int _sectorsCount;

    private Dictionary<Vector2Int, Sector> _sectors = new Dictionary<Vector2Int, Sector>();
    private Dictionary<Vector3Int, Cell> _allCells = new Dictionary<Vector3Int, Cell>();
    private float _clickStartTime;
    private bool _isHolding;
    private bool _flagSet;


    void Start()
    {
        mainCamera = Camera.main;
        _grid = GetComponent<Grid>();

        _cellGap.x = _grid.cellGap.x;
        _cellGap.y = _grid.cellGap.y;
    }

    private void Update()
    {
        UpdateVisibleSectors();
        
        if (Input.GetMouseButtonDown(0))
        {
            _clickStartTime = Time.time;
            _isHolding = true;
            _flagSet = false;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!_flagSet && _isHolding && Time.time - _clickStartTime < 0.5f)
            {
                GetSectorAtClick();
            }
            _isHolding = false;
        }

        if (_isHolding && !_flagSet && Time.time - _clickStartTime >= 0.5f)
        {
            SetFlagAtClick();
            _flagSet = true;
        }
    }
 

    public void GenerateFirstSectors(Sector startingSector, Cell startingCell)
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
                foreach (var cell in sector.Cells)
                {
                    Vector3Int cellWorldPosition = cell.Key + new Vector3Int((int)(sector.transform.position.x), (int)(sector.transform.position.y), 0);
                    _allCells[cellWorldPosition] = cell.Value;
                }

                GenerateMinesInSector(sector, startingCell);
                GenerateNumbers(sector);

                sector._isActive = true;

                if (sector.IsActive)
                {
                    _sectorsCount++;
                }                
            }
        }
    }

    public void GenerateSectors(Sector currentSector, Cell startingCell)
    {
        List<Sector> _sectorsToActivate = GetAdjacentSectors(currentSector);
        _sectorsToActivate.Add(currentSector);

        foreach (var sector in _sectorsToActivate)
        {
            if (!sector.IsActive)
            {
                foreach (var cell in sector.Cells)
                {

                    Vector3Int cellWorldPosition = cell.Key + new Vector3Int((int)(sector.transform.position.x), (int)(sector.transform.position.y), 0);
                    _allCells[cellWorldPosition] = cell.Value;
                }

                GenerateMinesInSector(sector, startingCell);
                GenerateNumbers(sector);

                sector._isActive = true;

                if (sector.IsActive)
                {
                    _sectorsCount++;
                }                                
            }
        }

        DrawSectors(currentSector);
    }


    private void GenerateMinesInSector(Sector sector, Cell startingCell)
    {
        int generatedMines = 0;
        while (generatedMines < CalculateDynamicMinesCount(_sectorsCount))
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

    private int CalculateDynamicMinesCount(int activeSectorCount)
    {
        int minMines = _minMinesCount;
        int maxMines = _maxMinesCount;

        // Вычисляем, сколько раз по 3 активных секторов открыто
        int incrementSteps = activeSectorCount / 3;

        // Обновляем пороги с учетом каждого шага
        minMines = Mathf.Min(minMines + incrementSteps, 15); // нижний порог не превышает 15
        maxMines = Mathf.Min(maxMines + incrementSteps * 2, 25); // верхний порог не превышает 25

        return Random.Range(minMines, maxMines);
    }

    public void GenerateNumbers(Sector sector)
    {
        foreach (var cell in sector.Cells)
        {
            if (cell.Value.CellState != CellState.Mine)
            {                
                int adjacentMines = CountAdjacentMines(cell.Value.CellPosition);
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

    private int CountAdjacentMines(Vector3Int cellPosition)
    {
        int count = 0;
        
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                Vector3Int adjacentPos = new Vector3Int(cellPosition.x + dx, cellPosition.y + dy, 0);

                if (_allCells.TryGetValue(adjacentPos, out Cell adjacentCell) && adjacentCell.CellState == CellState.Mine)
                {
                    count++;
                }
            }
        }

        return count;
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

    private void GetSectorAtClick()
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

            clickedSector.HandleCellClick(cellX, cellY);
        }
    }

    private void SetFlagAtClick()
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

            clickedSector.HandleFlag(cellX, cellY);
        }
    }

    public void Reveal(Sector currentSector, Cell cell)
    {
        if (cell.IsRevealed) return;
        if (cell.IsFlagged) return;

        switch (cell.CellState)
        {
            case CellState.Mine:
                Explode(cell);
                break;

            case CellState.Empty:
                StartCoroutine(Flood(currentSector, cell));

                //CheckWinCondition();
                break;

            default:
                cell.IsRevealed = true;
                cell.IsActive = true;

                //CheckWinCondition();
                break;
        }

        DrawSectors(currentSector);
    }

    public void Flag(Sector currentSector, Cell cell)
    {        
        if (cell.IsRevealed) return;

        cell.IsFlagged = !cell.IsFlagged;
        DrawSectors(currentSector);
    }

    private void Explode(Cell cell)
    {
        cell.IsRevealed = true;
        
    }

    private IEnumerator Flood(Sector currentSector, Cell cell)
    {        
        //if (IsGameOver) yield break;
        if (cell.IsRevealed) yield break;
        if (cell.CellState == CellState.Mine) yield break;

        cell.IsRevealed = true;
        //cell.IsActive = true;        

        DrawSectors(currentSector);

        yield return null;

        if (cell.CellState == CellState.Empty)
        {
            if (TryGetCell(cell.CellPosition.x - 1, cell.CellPosition.y, out Cell left))
            {
                StartCoroutine(Flood(currentSector, left));
            }
            if (TryGetCell(cell.CellPosition.x + 1, cell.CellPosition.y, out Cell right))
            {
                StartCoroutine(Flood(currentSector, right));
            }
            if (TryGetCell(cell.CellPosition.x, cell.CellPosition.y - 1, out Cell down))
            {
                StartCoroutine(Flood(currentSector, down));
            }
            if (TryGetCell(cell.CellPosition.x, cell.CellPosition.y + 1, out Cell up))
            {
                StartCoroutine(Flood(currentSector, up));
            }
            if (TryGetCell(cell.CellPosition.x - 1, cell.CellPosition.y - 1, out Cell downLeft))
            {
                StartCoroutine(Flood(currentSector, downLeft));
            }
            if (TryGetCell(cell.CellPosition.x + 1, cell.CellPosition.y - 1, out Cell downRight))
            {
                StartCoroutine(Flood(currentSector, downRight));
            }
            if (TryGetCell(cell.CellPosition.x - 1, cell.CellPosition.y + 1, out Cell upLeft))
            {
                StartCoroutine(Flood(currentSector, upLeft));
            }
            if (TryGetCell(cell.CellPosition.x + 1, cell.CellPosition.y + 1, out Cell upRight))
            {
                StartCoroutine(Flood(currentSector, upRight));
            }
        }       
    }

    public void CellsActivate(Cell cell)
    {
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

                if (TryGetCell(x, y, out Cell adjacent) && !adjacent.IsRevealed && !adjacent.IsActive)
                {

                    adjacent.IsActive = true;
                }
            }
        }
    }

    private void DrawSectors(Sector currentSector)
    {
        /*List<Sector> _sectorsToActivate = GetAdjacentSectors(currentSector);
        _sectorsToActivate.Add(currentSector);
        foreach (var sector in _sectorsToActivate)
        {
            if (sector.IsActive)
            {
                sector.DrawSector();
            }
        }*/
        foreach (var sector in _sectors.Values)
        {
            if (sector.IsActive)
            {
                sector.DrawSector();
            }
        }

    }

    public bool TryGetCell(int x, int y, out Cell cell)
    {
        cell = GetCell(new Vector3Int(x, y ,0));
        return cell != null;
    }

    private Cell GetCell(Vector3Int coordinates)
    {
        if (_allCells.ContainsKey(coordinates))
        {
            return _allCells[coordinates];
        }
        
        return null;
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

    private void UpdateVisibleSectors()
    {
        var currentSectorPosition = GetCurrentSectorPosition();
        var viewDistance = Mathf.CeilToInt((mainCamera.orthographicSize) / _sectorSize) + initialSectorsVisibleInRange;

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                var sectorPosition = new Vector2Int(currentSectorPosition.x + x, currentSectorPosition.y + y);
                CreateSector(sectorPosition);
            }
        }
    }

    private Vector2Int GetCurrentSectorPosition()
    {
        var x = mainCamera.transform.position.x / _sectorSize;
        var y = mainCamera.transform.position.y / _sectorSize;
        return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
    } 
}
