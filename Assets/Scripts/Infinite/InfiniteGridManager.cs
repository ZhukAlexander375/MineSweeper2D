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
    [Header("Camera Controller")]
    [SerializeField] private CameraController _cameraController;

    [Header("Settings")]
    [SerializeField] private Sector _sectorPrefab;
    [SerializeField] private int _minMinesCount;
    [SerializeField] private int _maxMinesCount;
    [SerializeField] private GameObject _flagPlaceParticle;
    [SerializeField] private GameObject _flagRemoveParticle;
    [SerializeField] private GameObject _targetRewardUIElement;
    [SerializeField] private GameObject _awardSpritePrefab;
    [SerializeField] private SectorBuyoutCostConfig _sectorBuyoutCostConfig;
    [SerializeField] private SectorRewardConfig sectorRewardConfig;

    public bool IsFirstClick;
    public bool IsGenerateEnabled;
    public int ExplodedMines;

    private int _sectorSize = 9;            ///
    private Camera mainCamera;    
    //private Grid _grid;
    private int initialSectorsVisibleInRange = 2;
    private int _sectorsCount;
    private Vector3 _lastClickPosition;

    private Dictionary<Vector2Int, Sector> _sectors = new Dictionary<Vector2Int, Sector>();
    private Dictionary<Vector3Int, InfiniteCell> _allCells = new Dictionary<Vector3Int, InfiniteCell>();
    private float _clickStartTime;
    private bool _isHolding;
    private bool _flagSet;
    private float _lastClickTime = -1f; // Время последнего клика
    private const float DoubleClickThreshold = 0.3f; // Порог для двойного клика (в секундах)

    private SaveManager _saveManager;
    private PlayerProgress _playerProgress;

    /// <summary>
    /// TO DO NORMAL CLASSES FOR MODES
    /// </summary>    
    private int currentRewardCount;

    private HashSet<Sector> _sectorsToRedraw = new HashSet<Sector>();
    private int _activeFloodCoroutines = 0;

    void Start()
    {
        _saveManager = SaveManager.Instance;
        _playerProgress = PlayerProgress.Instance;        

        mainCamera = Camera.main;
        //_grid = GetComponent<Grid>();

        if (GameModesManager.Instance.IsDownloadedInfiniteGame && !GameModesManager.Instance.IsNewInfiniteGame)
        {
            GameModesManager.Instance.IsDownloadedInfiniteGame = false;
            GameModesManager.Instance.IsNewInfiniteGame = false;
            LoadSavedGame();
        }

        else if (!GameModesManager.Instance.IsNewInfiniteGame && GameModesManager.Instance.IsDownloadedInfiniteGame)
        {
            GameModesManager.Instance.IsDownloadedInfiniteGame = false;
            GameModesManager.Instance.IsNewInfiniteGame = false;
            _saveManager.ClearSavedInfiniteGame();
        }

        CenterCameraOnSector();
        
    }

    private void Update()
    {
        UpdateVisibleSectors();

        if (_cameraController.IsCameraInteracting)
        {
            return;
        }

        // check click on ui or gamefield
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            return;
        }

        HandleGameInput();        
    }

    private void HandleGameInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float currentTime = Time.time;

            // Проверка на двойной клик
            if (currentTime - _lastClickTime <= DoubleClickThreshold)
            {
                GetSectorAtDoubleClick();
                _lastClickTime = -1f; // Сброс времени клика
            }
            else
            {
                _clickStartTime = currentTime;
                _isHolding = true;
                _flagSet = false;
                _lastClickTime = currentTime; // Обновляем время последнего клика
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
        
        if (!initializeSectors.Contains(startingSector))
        {
            initializeSectors.Insert(0, startingSector);
        }
        else
        {
            initializeSectors.Remove(startingSector);
            initializeSectors.Insert(0, startingSector);
        }

        foreach (var sector in initializeSectors)
        {
            if (!sector.IsActive)
            {
                foreach (var cell in sector.Cells)
                {
                    Vector3Int globalCellPosition = cell.Key + new Vector3Int((int)(sector.transform.position.x), (int)(sector.transform.position.y), 0);
                    _allCells[globalCellPosition] = cell.Value;                    
                }

                sector.IsActive = true;
                sector.IsPrizePlaced = true;

                if (sector.IsActive)
                {
                    _sectorsCount++;
                }

                GenerateMinesInSector(sector, startingCell);
                GenerateNumbers(sector);
            }
        }
        
        ///
        ///
        ///
        GameModesManager.Instance.IsDownloadedInfiniteGame = true;
        ///
        ///
        ///

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

                sector.IsActive = true;

                if (sector.IsActive)
                {
                    _sectorsCount++;
                }

                GenerateMinesInSector(sector, startingCell);
                GenerateNumbers(sector);                
            }
        }

        RedrawSectors();
    }


    private void GenerateMinesInSector(Sector sector, InfiniteCell startingCell)
    {
        int generatedMines = 0;
        int minesCount = CalculateProgressiveMinesCount();

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

    private int CalculateProgressiveMinesCount()
    {
        int baseMines = 7;
        int totalMines;

        if (_sectorsCount == 1)
        {
            totalMines = baseMines;
        }
        else
        {
            int additionalMines = (_sectorsCount - 2) / 2;
            totalMines = baseMines + 1 + additionalMines;
        }

        return Mathf.Min(totalMines, 50);
    }

    private int CalculateDynamicMinesCount(int activeSectorCount)
    {
        int minMines = _minMinesCount;
        int maxMines = _maxMinesCount;

        // Вычисляем, сколько раз по 2 активных секторов открыто
        int incrementSteps = activeSectorCount / 2;

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
                int adjacentMines = CountAdjacentMines(cell.Value.GlobalCellPosition);
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
        Vector3Int start = startingCell.GlobalCellPosition;
        Vector3Int pos = cell.GlobalCellPosition;

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
            RewardBonus(cell);
        }

        switch (cell.CellState)
        {
            case CellState.Mine:
                ///
                /// tut nachat'
                ///
                ExplodedMines++;
                UpdateExplodedMinesCount();
                currentSector.ExplodeSector(cell);
                currentSector.SetBuyoutCost(_sectorBuyoutCostConfig, ExplodedMines);

                break;

            case CellState.Empty:
                StartCoroutine(Flood(currentSector, cell, _sectorsToRedraw));

                //CheckWinCondition();
                currentSector.SectorCompletionCheck();
                break;

            default:
                cell.IsRevealed = true;

                SignalBus.Fire<CellRevealedSignal>();

                cell.IsActive = true;

                //CheckWinCondition();
                currentSector.SectorCompletionCheck();
                break;
        }

        _lastClickPosition = cell.GlobalCellPosition;

        RedrawSectors();
    }

    public void Flag(Sector currentSector, InfiniteCell cell)
    {
        if (cell.IsRevealed) return;

        bool isPlacingFlag = !cell.IsFlagged;
        cell.IsFlagged = !cell.IsFlagged;

        SignalBus.Fire(new FlagPlacingSignal(isPlacingFlag));

        if (isPlacingFlag && cell.IsAward)
        {
            RewardBonus(cell);
        }

        InstantiateParticleAtCell(isPlacingFlag ? _flagPlaceParticle : _flagRemoveParticle, cell);

        _lastClickPosition = cell.GlobalCellPosition;

        // Вибрация и перерисовка
        if (GameSettingsManager.Instance.IsVibrationEnabled)
        {
            VibrateOnAction();
        }

        RedrawSectors();

        currentSector.SectorCompletionCheck();
    }

    private void InstantiateParticleAtCell(GameObject particlePrefab, InfiniteCell cell)
    {
        Vector3 worldPosition = cell.GlobalCellPosition;
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

                        Vector3Int position = currentCell.GlobalCellPosition + new Vector3Int(adjacentX, adjacentY, 0);

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

        RedrawSectors();
    }


    public int CountAdjacentFlags(InfiniteCell cell)
    {
        int flagCount = 0;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                Vector3Int position = cell.GlobalCellPosition + new Vector3Int(dx, dy, 0);

                if (_allCells.TryGetValue(position, out InfiniteCell adjacentCell) && adjacentCell.IsFlagged)
                {
                    flagCount++;
                }
            }
        }

        return flagCount;
    }

    private IEnumerator Flood(Sector currentSector, InfiniteCell cell, HashSet<Sector> affectedSectors)
    {
        if (cell.IsRevealed || cell.CellState == CellState.Mine) yield break;

        _activeFloodCoroutines++;

        if (cell.IsAward)
        {
            RewardBonus(cell);
        }

        cell.IsRevealed = true;

        if (cell.IsRevealed)
        {
            SignalBus.Fire<CellRevealedSignal>();
        }

        affectedSectors.Add(currentSector);

        currentSector.SectorCompletionCheck();

        yield return null;

        if (cell.CellState == CellState.Empty)
        {
            foreach (var neighbor in GetNeighbors(cell))
            {
                StartCoroutine(Flood(currentSector, neighbor, affectedSectors));
            }
        }

        _activeFloodCoroutines--;

        if (_activeFloodCoroutines == 0)
        {
            RedrawSectors();
        }
    }

    private IEnumerable<InfiniteCell> GetNeighbors(InfiniteCell cell)
    {
        var directions = new[]
        {
            (x: -1, y: 0), (x: 1, y: 0), (x: 0, y: -1), (x: 0, y: 1),
            (x: -1, y: -1), (x: 1, y: -1), (x: -1, y: 1), (x: 1, y: 1)
        };

        foreach (var (dx, dy) in directions)
        {
            if (TryGetCell(cell.GlobalCellPosition.x + dx, cell.GlobalCellPosition.y + dy, out var neighbor))
            {
                yield return neighbor;
            }
        }
    }

    private void RewardBonus(InfiniteCell cell)
    {
        if (!cell.IsAward) return;
        currentRewardCount++;
        int currentReward = CalculateCurrentReward(currentRewardCount);

        SignalBus.Fire(new OnGameRewardSignal(0, currentReward));
        Debug.Log($"Номер награды: {currentRewardCount}, награда: {currentReward}");
        cell.IsAward = false;

        MoveAwardSprite(cell, _targetRewardUIElement);
    }

    private int CalculateCurrentReward(int collectedRewards)
    {
        int rewardLevelIndex = Mathf.Min(collectedRewards - 1, sectorRewardConfig.RewardLevels.Count - 1);
        RewardLevel currentRewardLevel = sectorRewardConfig.RewardLevels[rewardLevelIndex];
        
        float randomMultiplier = Random.Range(1f - currentRewardLevel.RandomizationCoefficient, 1f + currentRewardLevel.RandomizationCoefficient);
        int reward = Mathf.RoundToInt(currentRewardLevel.CurrencyAmount * randomMultiplier);
        
        reward = Mathf.Clamp(reward, currentRewardLevel.MinReward, currentRewardLevel.MaxReward);

        return reward;
    }

    private void MoveAwardSprite(InfiniteCell cell, GameObject targetRewardUIElement)
    {
        Vector3 startPosition = cell.GlobalCellPosition;

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

                int x = cell.GlobalCellPosition.x + adjacentX;
                int y = cell.GlobalCellPosition.y + adjacentY;

                if (TryGetCell(x, y, out InfiniteCell adjacent) && !adjacent.IsRevealed && !adjacent.IsActive)
                {

                    adjacent.IsActive = true;
                }
            }
        }
    }

    private void RedrawSectors()
    {
        foreach (var sector in _sectors.Values)
        {
            if (sector.IsActive)
            {
                sector.RedrawSector();
            }
        }
    }

    public bool TryGetCell(int x, int y, out InfiniteCell cell)
    {
        cell = GetCell(new Vector3Int(x, y, 0));
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

    private void CenterCameraOnSector()
    {
        var sectorX = 0;
        var sectorY = 0;

        var sectorCenterX = sectorX * _sectorSize + _sectorSize / 2f;
        var sectorCenterY = sectorY * _sectorSize + _sectorSize / 2f;

        mainCamera.transform.position = new Vector3(sectorCenterX, sectorCenterY, mainCamera.transform.position.z);
    }        

    private void UpdateVisibleSectors()
    {
        var globalSectorPosition = GetCurrentSectorPosition();       //sector's global position -16 -8  --> sector position -2 -1 
        var viewDistance = Mathf.CeilToInt((mainCamera.orthographicSize) / _sectorSize) + initialSectorsVisibleInRange;

        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                var sectorPosition = new Vector2Int(globalSectorPosition.x + x, globalSectorPosition.y + y);
                
                if (!_sectors.ContainsKey(sectorPosition))
                {
                    CreateSector(sectorPosition);       //tut sector position -2 -1 i t.d.                 
                }
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

            _sectors.Add(position, newSector);       //tut sector position -2 -1 i t.d.( / 8)
            //Debug.Log($"в апдейте: {newSector} position: {position}");
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

    private void UpdateExplodedMinesCount()
    {
        PlayerProgress.Instance.UpdateExplodedMinesCount(ExplodedMines);
    }

    private void OnApplicationQuit()
    {
        SaveCurrentGame();
        SavePlayerProgress();

        if (!IsFirstClick)
        {
            GameModesManager.Instance.IsDownloadedInfiniteGame = false;
            GameModesManager.Instance.IsNewInfiniteGame = true;
        }

        else
        {
            GameModesManager.Instance.IsDownloadedInfiniteGame = true;
            GameModesManager.Instance.IsNewInfiniteGame = false;
        }
    }

    public void SaveCurrentGame()
    {
        var activeSectors = _sectors.Values.Where(s => s.IsActive).ToList();
        var sectorDataList = activeSectors.Select(sector => sector.SaveSectorData()).ToList();
        _saveManager.SaveInfiniteGame(sectorDataList);
    }

    private void SavePlayerProgress()
    {
        _playerProgress.SavePlayerProgress();        
    }

    public void LoadSavedGame()
    {
        if (_saveManager.HasSavedData())
        {
            if (!SaveManager.Instance.HasSavedData())
            {
                Debug.LogWarning("No saved game data found.");
                return;
            }
            
            List<SectorData> loadedSectors = SaveManager.Instance.LoadSavedSectors();

            foreach (var sectorData in loadedSectors)
            {
                var position = new Vector2Int(sectorData.SectorPosition.x, sectorData.SectorPosition.y);

                if (_sectors.ContainsKey(position))
                {
                    Debug.Log($"Sector at {position} already exists. Skipping creation.");
                    continue;
                }

                Sector sector = CreateSectorFromData(sectorData);
                sector.InitializeCellsFromData(sectorData.Cells, this);
                AddCellsToAllCells(sector);

                sector.IsLOADED = false;
            }

            //Debug.Log("Game loaded successfully.");
            IsGenerateEnabled = true;            
            IsFirstClick = true;        ////???????????????????????????????????????
        }        
    }

    private Sector CreateSectorFromData(SectorData sectorData)
    {
        var logicalPosition = new Vector2Int(sectorData.SectorPosition.x, sectorData.SectorPosition.y); // load with sector position -2 -1 etc

        if (!_sectors.TryGetValue(logicalPosition, out Sector sector))
        {
            var sectorWorldPosition = new Vector3(
                logicalPosition.x * _sectorSize,
                logicalPosition.y * _sectorSize,
                0);

            var newSector = Instantiate(_sectorPrefab, sectorWorldPosition, Quaternion.identity, transform);
            newSector.SetManager(this);

            if (ThemeManager.Instance != null)
            {
                newSector.TryApplyTheme(ThemeManager.Instance.CurrentThemeIndex);
            }

            _sectors.Add(logicalPosition, newSector);
            sector = newSector;
            //Debug.Log($"при загрузке: {newSector} position: {logicalPosition}");
        }
        
        sector.IsActive = sectorData.IsActive;
        sector.IsFirstCellActivated = sectorData.IsFirstCellActivated;
        sector.IsExploded = sectorData.IsExploded;
        sector.IsPrizePlaced = sectorData.IsPrizePlaced;
        sector.IsLOADED = true;

        if (sector.IsActive)
        {
            _sectorsCount++;
        }
        //Debug.Log($"Секторов: {_sectorsCount}");
        return sector;
    }

    private void AddCellsToAllCells(Sector sector)
    {
        foreach (var cell in sector.Cells)
        {
            Vector3Int cellWorldPosition = cell.Key + new Vector3Int((int)(sector.transform.position.x), (int)(sector.transform.position.y), 0);
            //Debug.Log(cellWorldPosition);
            _allCells[cellWorldPosition] = cell.Value;
        }
    }
}
