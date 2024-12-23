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
    private float _lastClickTime = -1f; // ����� ���������� �����
    private const float DoubleClickThreshold = 0.3f; // ����� ��� �������� ����� (� ��������)

    private SaveManager _saveManager;
    private PlayerProgress _playerProgress;
    private GameManager _gameManager;
    private IStatisticController _statisticController;
    private int _currentRewardLevel;
    private int _currentSectorBuyoutLevel;



    void Start()
    {
        _saveManager = SaveManager.Instance;
        _playerProgress = PlayerProgress.Instance;
        _gameManager = GameManager.Instance;
        _statisticController = GameManager.Instance.CurrentStatisticController;

        mainCamera = Camera.main;

        CheckGameStart();        
        SetCurrentRewardLevel();
        SetCurrentSectorBuyoutLevel();
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

    private void CheckGameStart()
    {
        switch (GameManager.Instance.CurrentGameMode)
        {
            case GameMode.SimpleInfinite:
                if (_gameManager.SimpleInfiniteStats.IsGameStarted)
                {
                    LoadSavedGame();
                    SignalBus.Fire<LoadCompletedSignal>();
                }
                else
                {
                    StartNewGame();
                }
                break;

            case GameMode.Hardcore:
                if (_gameManager.HardcoreStats.IsGameStarted)
                {
                    LoadSavedGame();
                    SignalBus.Fire<LoadCompletedSignal>();
                }
                else
                {
                    StartNewGame();
                }
                break;

            case GameMode.TimeTrial:
                if (_gameManager.TimeTrialStats.IsGameStarted)
                {
                    LoadSavedGame();
                    SignalBus.Fire<LoadCompletedSignal>();
                }
                else
                {
                    StartNewGame();
                }
                break;

            default:
                Debug.LogWarning("Unknown game mode. No saved data.");
                return;
        }
    }

    private void StartNewGame()
    {
        _gameManager.ResetCurrentModeStatistic();
        //_currentGameModeData.InitializeNewGame();
        SignalBus.Fire<LoadCompletedSignal>();
    }

    private void SetCurrentRewardLevel() //MB � ����� ����
    {
        _currentRewardLevel = _statisticController.RewardLevel;
        //Debug.Log($"Reward Level in  Mode: {_currentRewardLevel}");        
    }

    private void SetCurrentSectorBuyoutLevel()
    {
        _currentSectorBuyoutLevel = _statisticController.SectorBuyoutCostLevel;
        //Debug.Log($"_currentSectorBuyoutLevel in Simple Infinite Mode: {_currentSectorBuyoutLevel}");        
    }

    private void HandleGameInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float currentTime = Time.time;

            // �������� �� ������� ����
            if (currentTime - _lastClickTime <= DoubleClickThreshold)
            {
                GetSectorAtDoubleClick();
                _lastClickTime = -1f; // ����� ������� �����
            }
            else
            {
                _clickStartTime = currentTime;
                _isHolding = true;
                _flagSet = false;
                _lastClickTime = currentTime; // ��������� ����� ���������� �����
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
        //GameManager.Instance.IsDownloadedInfiniteGame = true;
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

        // ���������, ������� ��� �� 2 �������� �������� �������
        int incrementSteps = activeSectorCount / 2;

        // ��������� ������ � ������ ������� ����
        minMines = Mathf.Min(minMines + incrementSteps, 15); // ������ ����� �� ��������� 15
        maxMines = Mathf.Min(maxMines + incrementSteps * 2, 25); // ������� ����� �� ��������� 25

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

                UpdateExplodedMinesCount();
                UpdateSectorBuyoutLevel();

                currentSector.SetBuyoutCost(_sectorBuyoutCostConfig, _currentSectorBuyoutLevel);
                currentSector.ExplodeSector(cell);                

                break;

            case CellState.Empty:
                StartCoroutine(Flood(currentSector, cell));

                //CheckWinCondition();
                SectorCompletionCheck(currentSector);
                break;

            default:
                cell.IsRevealed = true;

                UpdateOpenedCells();
                SignalBus.Fire<CellRevealedSignal>();

                cell.IsActive = true;

                //CheckWinCondition();
                SectorCompletionCheck(currentSector);
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

        UpdateFlagsCount(isPlacingFlag);
        SignalBus.Fire(new FlagPlacingSignal(isPlacingFlag));

        if (isPlacingFlag && cell.IsAward)
        {
            RewardBonus(cell);
        }

        InstantiateParticleAtCell(isPlacingFlag ? _flagPlaceParticle : _flagRemoveParticle, cell);

        _lastClickPosition = cell.GlobalCellPosition;

        // �������� � �����������
        if (GameSettingsManager.Instance.IsVibrationEnabled)
        {
            VibrateOnAction();
        }

        RedrawSectors();

        SectorCompletionCheck(currentSector);
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

    private IEnumerator Flood(Sector currentSector, InfiniteCell cell)
    {
        //if (IsGameOver) yield break;
        if (cell.IsRevealed) yield break;
        if (cell.CellState == CellState.Mine) yield break;

        if (cell.IsAward)
        {
            RewardBonus(cell);
        }

        cell.IsRevealed = true;
        //cell.IsActive = true;        

        if (cell.IsRevealed)
        {
            UpdateOpenedCells();
            SignalBus.Fire<CellRevealedSignal>();
        }
        
        RedrawSectors();

        SectorCompletionCheck(currentSector);

        yield return null;

        if (cell.CellState == CellState.Empty)
        {
            if (TryGetCell(cell.GlobalCellPosition.x - 1, cell.GlobalCellPosition.y, out InfiniteCell left))
            {
                StartCoroutine(Flood(currentSector, left));
            }
            if (TryGetCell(cell.GlobalCellPosition.x + 1, cell.GlobalCellPosition.y, out InfiniteCell right))
            {
                StartCoroutine(Flood(currentSector, right));
            }
            if (TryGetCell(cell.GlobalCellPosition.x, cell.GlobalCellPosition.y - 1, out InfiniteCell down))
            {
                StartCoroutine(Flood(currentSector, down));
            }
            if (TryGetCell(cell.GlobalCellPosition.x, cell.GlobalCellPosition.y + 1, out InfiniteCell up))
            {
                StartCoroutine(Flood(currentSector, up));
            }
            if (TryGetCell(cell.GlobalCellPosition.x - 1, cell.GlobalCellPosition.y - 1, out InfiniteCell downLeft))
            {
                StartCoroutine(Flood(currentSector, downLeft));
            }
            if (TryGetCell(cell.GlobalCellPosition.x + 1, cell.GlobalCellPosition.y - 1, out InfiniteCell downRight))
            {
                StartCoroutine(Flood(currentSector, downRight));
            }
            if (TryGetCell(cell.GlobalCellPosition.x - 1, cell.GlobalCellPosition.y + 1, out InfiniteCell upLeft))
            {
                StartCoroutine(Flood(currentSector, upLeft));
            }
            if (TryGetCell(cell.GlobalCellPosition.x + 1, cell.GlobalCellPosition.y + 1, out InfiniteCell upRight))
            {
                StartCoroutine(Flood(currentSector, upRight));
            }
        }
    }

    private void RewardBonus(InfiniteCell cell)
    {
        if (!cell.IsAward) return;

        _statisticController.IncrementRewardLevel();
        SetCurrentRewardLevel();
        
        //int currentReward = CalculateCurrentReward(_currentGameModeData.GetRewardLevel());

        //SignalBus.Fire(new OnGameRewardSignal(0, currentReward));
        //Debug.Log($"����� �������: {_currentRewardLevel}, �������: {currentReward}");
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
            //Debug.Log($"� �������: {newSector} position: {position}");
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

    private void UpdateOpenedCells()
    {
        _statisticController.IncrementOpenedCells();
    }

    private void UpdateFlagsCount(bool isPlacingFlag)
    {
        _statisticController.IncrementPlacedFlags(isPlacingFlag);
    }

    private void UpdateExplodedMinesCount()
    {
        _statisticController.IncrementExplodedMines();
        
    }

    private void UpdateSectorBuyoutLevel()
    {
        _statisticController.IncrementSectorBuyoutIndex();
        Debug.Log(_statisticController.SectorBuyoutCostLevel);
        SetCurrentSectorBuyoutLevel();
    }

    private void SectorCompletionCheck(Sector currentSector)
    {
        currentSector.SectorCompletionCheck();

        if (currentSector.IsSectorCompleted)
        {
            _statisticController.IncrementCompletedSectors();
        }
    }

    private void OnApplicationQuit()
    {
        SaveCurrentGame();
        SavePlayerProgress();

        if (!IsFirstClick)
        {

        }
        else
        {

        }
    }

    public void SaveCurrentGame()
    {
        var activeSectors = _sectors.Values.Where(s => s.IsActive).ToList();
        var sectorDataList = activeSectors.Select(sector => sector.SaveSectorData()).ToList();

        switch (GameManager.Instance.CurrentGameMode)
        {
            case GameMode.SimpleInfinite:
                _saveManager.SaveSimpleInfiniteGame(sectorDataList, GameManager.Instance.SimpleInfiniteStats);
                break;

            case GameMode.Hardcore:
                _saveManager.SaveHardcoreGame(sectorDataList, GameManager.Instance.HardcoreStats);
                break;

            case GameMode.TimeTrial:
                _saveManager.SaveTimeTrialGame(sectorDataList, GameManager.Instance.TimeTrialStats);
                break;

            default:
                Debug.LogError("Unknown game mode. Cannot save.");
                break;
    }
}

    private void SavePlayerProgress()
    {
        _playerProgress.SavePlayerProgress();        
    }

    public void LoadSavedGame()
    {
        if (!_saveManager.HasSavedData(GameManager.Instance.CurrentGameMode))
        {
            StartNewGame();
            return;
        }

        List<SectorData> loadedSectors;        

        switch (GameManager.Instance.CurrentGameMode)
        {
            case GameMode.SimpleInfinite:
                var simpleSectors = _saveManager.LoadSimpleInfiniteGameGrid();
                loadedSectors = simpleSectors;
                break;

            case GameMode.Hardcore:
                var hardcoreSectors = _saveManager.LoadHardcoreGameGrid();
                loadedSectors = hardcoreSectors;
                break;

            case GameMode.TimeTrial:
                var timeTrialSectors = _saveManager.LoadTimeTrialGameGrid();
                loadedSectors = timeTrialSectors;
                break;

            default:
                Debug.LogWarning("Unknown game mode. No saved data.");
                return;
        }

        if (loadedSectors != null)
        {
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
        }

        /*if (loadedGameModeData != null)
        {
            _currentGameModeData = loadedGameModeData;
            GameManager.Instance.ApplyGameModeData(loadedGameModeData);
        }*/

        IsGenerateEnabled = true;            
        IsFirstClick = true;        ////???????????????????????????????????????
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
            //Debug.Log($"��� ��������: {newSector} position: {logicalPosition}");
        }
        
        sector.IsActive = sectorData.IsActive;
        sector.IsFirstCellActivated = sectorData.IsFirstCellActivated;
        sector.IsExploded = sectorData.IsExploded;
        sector.IsPrizePlaced = sectorData.IsPrizePlaced;
        sector.IsLOADED = true;
        sector.IsSectorCompleted = sectorData.IsSectorCompleted;
        sector.CurrentBuyoutCost = sectorData.CurrentBuyoutCost;


        if (sector.IsActive)
        {
            _sectorsCount++;
        }
        //Debug.Log($"��������: {_sectorsCount}");
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
