using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;


public class InfiniteGridManager : MonoBehaviour
{
    public int SectorSize => _sectorSize;

    [Header("Settings")]
    [SerializeField] private Sector _sectorPrefab;
    [SerializeField] private int _minMinesCount;
    [SerializeField] private int _maxMinesCount;
    [SerializeField] private GameObject _flagPlaceParticle;
    [SerializeField] private GameObject _flagRemoveParticle;
    [SerializeField] private GameObject _targetRewardUIElement;
    [SerializeField] private GameObject _awardSpritePrefab;

    public bool IsFirstClick;

    private int _sectorSize = 8;            ///
    private Camera mainCamera;
    private Grid _grid;
    //private Vector2 _cellGap;
    private int initialSectorsVisibleInRange = 2;
    private int _sectorsCount;
    private Vector3 _lastClickPosition;

    private Dictionary<Vector2Int, Sector> _sectors = new Dictionary<Vector2Int, Sector>();    
    private Dictionary<Vector3Int, InfiniteCell> _allCells = new Dictionary<Vector3Int, InfiniteCell>();
    private float _clickStartTime;
    private bool _isHolding;
    private bool _flagSet;
    private float _lastClickTime = -1f; // ¬рем€ последнего клика
    private const float DoubleClickThreshold = 0.3f; // ѕорог дл€ двойного клика (в секундах)


    void Start()
    {
        mainCamera = Camera.main;
        _grid = GetComponent<Grid>();
        
        //_cellGap.x = _grid.cellGap.x;
        //_cellGap.y = _grid.cellGap.y;
    }

    private void Update()
    {
        UpdateVisibleSectors();

        // check click on ui or gamefield
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            float currentTime = Time.time;

            // ѕроверка на двойной клик
            if (currentTime - _lastClickTime <= DoubleClickThreshold)
            {
                GetSectorAtDoubleClick();
                _lastClickTime = -1f; // —брос времени клика
            }
            else
            {
                _clickStartTime = currentTime;
                _isHolding = true;
                _flagSet = false;
                _lastClickTime = currentTime; // ќбновл€ем врем€ последнего клика
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!_flagSet && _isHolding && Time.time - _clickStartTime < GameSettingsManager.Instance.HoldTime)
            {                
                GetSectorAtClick();
            }
            _isHolding = false;
        }

        if (_isHolding && !_flagSet && Time.time - _clickStartTime >= GameSettingsManager.Instance.HoldTime)
        {
            SetSectorForFlagAtClick();
            _flagSet = true;
        }
    }


    public void GenerateFirstSectors(Sector startingSector, InfiniteCell startingCell)
    {
        if (IsFirstClick)
        {
            return;
        }

        IsFirstClick = true;

        List<Sector> initializeSectors = GetAdjacentSectors(startingSector);
        //initializeSectors.Add(startingSector);
        
        foreach (var sector in initializeSectors)
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

                sector.IsActive = true;

                if (sector.IsActive)
                {
                    _sectorsCount++;
                }

                sector.IsPrizePlaced = true;
            }
        }

        GenerateSectorAwards(); 
    }

    public void GenerateSectors(Sector currentSector, InfiniteCell startingCell)
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
                
                sector.IsActive = true;

                if (sector.IsActive)
                {
                    _sectorsCount++;
                }
            }
        }

        DrawSectors();
    }


    private void GenerateMinesInSector(Sector sector, InfiniteCell startingCell)
    {
        int generatedMines = 0;
        int minesCount = CalculateDynamicMinesCount(_sectorsCount);

        while (generatedMines < minesCount)
        {
            Vector3Int randomPosition = new Vector3Int(
                Random.Range(0, _sectorSize),
                Random.Range(0, _sectorSize),
                0
            );

            InfiniteCell cell = sector.GetCell(randomPosition);

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

        // ¬ычисл€ем, сколько раз по 2 активных секторов открыто
        int incrementSteps = activeSectorCount / 2;

        // ќбновл€ем пороги с учетом каждого шага
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

                if (_allCells.TryGetValue(adjacentPos, out InfiniteCell adjacentCell) && adjacentCell.CellState == CellState.Mine)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private void GenerateSectorAwards()
    {
        if (IsFirstClick)
        {
            foreach (var sector in _sectors.Values)
            {
                sector.GenerateAward();
            }
        }        
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

    private bool IsAdjacent(InfiniteCell startingCell, InfiniteCell cell)
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

    private void SetSectorForFlagAtClick()
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

            clickedSector.HandleCellFlag(cellX, cellY);
        }
    }

    private void GetSectorAtDoubleClick()
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

            clickedSector.HandleChord(cellX, cellY);
        }
    }

    public void Reveal(Sector currentSector, InfiniteCell cell)
    {        
        if (cell.IsRevealed) return;
        if (cell.IsFlagged) return;

        if (cell.IsAward)
        {
            AwardBonus(cell);
        }

        switch (cell.CellState)
        {
            case CellState.Mine:
                Explode(currentSector, cell);
                break;

            case CellState.Empty:
                StartCoroutine(Flood(currentSector, cell));

                //CheckWinCondition();
                break;

            default:                
                cell.IsRevealed = true;

                SignalBus.Fire<CellRevealedSignal>();

                cell.IsActive = true;
                
                //CheckWinCondition();
                break;
        }       

        _lastClickPosition = cell.CellPosition;

        DrawSectors();
    }

    public void Flag(Sector currentSector, InfiniteCell cell)
    {
        if (cell.IsRevealed) return;

        bool isPlacingFlag = !cell.IsFlagged;
        cell.IsFlagged = !cell.IsFlagged;

        SignalBus.Fire(new FlagPlacingSignal(isPlacingFlag));

        if (isPlacingFlag && cell.IsAward)
        {
            AwardBonus(cell);
        }

        InstantiateParticleAtCell(isPlacingFlag ? _flagPlaceParticle : _flagRemoveParticle, cell);

        _lastClickPosition = cell.CellPosition;

        // ¬ибраци€ и перерисовка
        if (GameSettingsManager.Instance.IsVibrationEnabled)
        {
            VibrateOnAction();
        }
            
        DrawSectors();
    }

    private void InstantiateParticleAtCell(GameObject particlePrefab, InfiniteCell cell)
    {
        Vector3 worldPosition = cell.CellPosition;
        GameObject particleInstance = Instantiate(particlePrefab, worldPosition, Quaternion.identity);
        Destroy(particleInstance, 2f);
    }

    private void VibrateOnAction()
    {
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }

    public void Unchord(Sector currentSector, InfiniteCell currentCell)
    {
        foreach (var cell in _allCells.Values)
        {
            cell.Chorded = false;
        }
        
        List<InfiniteCell> cellsToReveal = new List<InfiniteCell>();
        
        if (currentCell.IsRevealed && currentCell.CellState == CellState.Number)
        {
            int adjacentFlags = CountAdjacentFlags(currentCell);

            if (adjacentFlags >= currentCell.CellNumber)
            {
                cellsToReveal.Add(currentCell);
                
                for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
                {
                    for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
                    {
                        if (adjacentX == 0 && adjacentY == 0)
                        {
                            continue;
                        }

                        Vector3Int position = currentCell.CellPosition + new Vector3Int(adjacentX, adjacentY, 0);

                        if (_allCells.TryGetValue(position, out InfiniteCell adjacentCell))
                        {
                            if (!adjacentCell.IsRevealed && !adjacentCell.IsFlagged)
                            {
                                cellsToReveal.Add(adjacentCell);
                            }
                        }
                    }
                }
            }
        }
        
        foreach (var cell in cellsToReveal)
        {
            Reveal(currentSector, cell);
        }

        DrawSectors();
    }


    public int CountAdjacentFlags(InfiniteCell cell)
    {
        int flagCount = 0;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                Vector3Int position = cell.CellPosition + new Vector3Int(dx, dy, 0);

                if (_allCells.TryGetValue(position, out InfiniteCell adjacentCell) && adjacentCell.IsFlagged)
                {
                    flagCount++;                    
                }
            }
        }

        return flagCount;
    }

    private void Explode(Sector currentSector, InfiniteCell cell)
    {        
        cell.IsRevealed = true;               
        currentSector.CloseSector();
        // Add logic of lose in sector/game?
    }

    private IEnumerator Flood(Sector currentSector, InfiniteCell cell)
    {        
        //if (IsGameOver) yield break;
        if (cell.IsRevealed) yield break;
        if (cell.CellState == CellState.Mine) yield break;

        if (cell.IsAward)
        {
            AwardBonus(cell);
        }

        cell.IsRevealed = true;
        //cell.IsActive = true;        

        if (cell.IsRevealed)
        {
            SignalBus.Fire<CellRevealedSignal>();
        }

        DrawSectors();

        yield return null;

        if (cell.CellState == CellState.Empty)
        {
            if (TryGetCell(cell.CellPosition.x - 1, cell.CellPosition.y, out InfiniteCell left))
            {
                StartCoroutine(Flood(currentSector, left));
            }
            if (TryGetCell(cell.CellPosition.x + 1, cell.CellPosition.y, out InfiniteCell right))
            {
                StartCoroutine(Flood(currentSector, right));
            }
            if (TryGetCell(cell.CellPosition.x, cell.CellPosition.y - 1, out InfiniteCell down))
            {
                StartCoroutine(Flood(currentSector, down));
            }
            if (TryGetCell(cell.CellPosition.x, cell.CellPosition.y + 1, out InfiniteCell up))
            {
                StartCoroutine(Flood(currentSector, up));
            }
            if (TryGetCell(cell.CellPosition.x - 1, cell.CellPosition.y - 1, out InfiniteCell downLeft))
            {
                StartCoroutine(Flood(currentSector, downLeft));
            }
            if (TryGetCell(cell.CellPosition.x + 1, cell.CellPosition.y - 1, out InfiniteCell downRight))
            {
                StartCoroutine(Flood(currentSector, downRight));
            }
            if (TryGetCell(cell.CellPosition.x - 1, cell.CellPosition.y + 1, out InfiniteCell upLeft))
            {
                StartCoroutine(Flood(currentSector, upLeft));
            }
            if (TryGetCell(cell.CellPosition.x + 1, cell.CellPosition.y + 1, out InfiniteCell upRight))
            {
                StartCoroutine(Flood(currentSector, upRight));
            }
        }       
    }

    private void AwardBonus(InfiniteCell cell)
    {
        if (!cell.IsAward) return;

        SignalBus.Fire(new OnGameRewardSignal(0, 1));
        cell.IsAward = false;

        MoveAwardSprite(cell, _targetRewardUIElement);
    }

    private void MoveAwardSprite(InfiniteCell cell, GameObject targetRewardUIElement)
    {
        Vector3 startPosition = cell.CellPosition;
               
        GameObject awardSprite = Instantiate(_awardSpritePrefab, startPosition, Quaternion.identity);
        awardSprite.transform.localScale = Vector3.one;

        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(
            _targetRewardUIElement.transform.position.x,
            _targetRewardUIElement.transform.position.y,
            Camera.main.nearClipPlane));
                
        awardSprite.transform.DOMove(targetPosition, 1.5f)
            .SetEase(Ease.InOutQuad) 
            .OnComplete(() =>
            {                
                Destroy(awardSprite);
            });
       
       // awardSprite.transform.DOScale(0f, 1f).SetEase(Ease.InOutQuad);
                
       // var spriteRenderer = awardSprite.GetComponent<SpriteRenderer>();
        //spriteRenderer.DOFade(0f, 1f);
    }


    public void CellsActivate(InfiniteCell cell)
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

                if (TryGetCell(x, y, out InfiniteCell adjacent) && !adjacent.IsRevealed && !adjacent.IsActive)
                {

                    adjacent.IsActive = true;
                }
            }
        }
    }

    private void DrawSectors()
    {
        foreach (var sector in _sectors.Values)
        {
            if (sector.IsActive)
            {
                sector.DrawSector();
            }
        }
    }

    public bool TryGetCell(int x, int y, out InfiniteCell cell)
    {
        cell = GetCell(new Vector3Int(x, y ,0));
        return cell != null;
    }

    private InfiniteCell GetCell(Vector3Int coordinates)
    {
        if (_allCells.ContainsKey(coordinates))
        {
            return _allCells[coordinates];
        }
        
        return null;
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

    private void CreateSector(Vector2Int position)
    {
        if (!_sectors.ContainsKey(position))
        {
            var sectorWorldPosition = new Vector3(position.x * _sectorSize, position.y * _sectorSize, 0);
            var newSector = Instantiate(_sectorPrefab, sectorWorldPosition, Quaternion.identity, transform);
            newSector.SetManager(this);                

            if (ThemeManager.Instance != null)
            {
                newSector.TryApplyTheme(ThemeManager.Instance.CurrentThemeIndex);
            }

            _sectors.Add(position, newSector);            
        }
    }

    private Vector2Int GetCurrentSectorPosition()
    {
        var x = mainCamera.transform.position.x / _sectorSize;
        var y = mainCamera.transform.position.y / _sectorSize;
        return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
    }

    // ! ! ! ! ADD IN INSPECTOR
    public void ReturnCameraToLastClick()
    {
        if (_lastClickPosition != Vector3.zero)
        {
            mainCamera.transform.position = new Vector3(_lastClickPosition.x, _lastClickPosition.y, mainCamera.transform.position.z);
        }
    }
}
