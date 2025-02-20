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
    //[SerializeField] private float _postCameraInteractionDelay = 0.15f;

    [Header("Settings")]
    [SerializeField] private Sector _sectorPrefab;
    [SerializeField] private int _minMinesCount;
    [SerializeField] private int _maxMinesCount;
    [SerializeField] private GameObject _flagPlaceParticle;
    [SerializeField] private GameObject _flagRemoveParticle;
    [SerializeField] private GameObject _targetRewardUIElement;
    [SerializeField] private GameObject _awardSpritePrefab;
    [SerializeField] private SectorBuyoutCostConfig _sectorBuyoutCostConfig;
    [SerializeField] private SectorRewardConfig _sectorRewardConfig;
    [SerializeField] private MinesConfig _minesConfig;    

    public bool IsFirstClick;
    public bool IsGenerateEnabled;    

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
    private GameManager _gameManager;
    private IStatisticController _statisticController;
    private int _currentRewardLevel;
    private int _currentSectorBuyoutLevel;
       

    private HashSet<Sector> _sectorsToRedraw = new HashSet<Sector>();
    private int _activeFloodCoroutines = 0;
    //private float _lastCameraInteractionTime = 0f;
    private bool IsInputLocked;

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
            IsInputLocked = true;
            _isHolding = false;
            return;
        }

        if (_cameraController.HasFinishedInteracting)
        {
            _cameraController.ResetFinishedInteracting();
            if (!Input.GetMouseButton(0) && Input.touchCount == 0)
            {
                IsInputLocked = false;
            }

        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            return;
        }

        IsInputLocked = false;

        if (!IsInputLocked)
        {
            HandleGameInput();
        }
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

                    if (_gameManager.HardcoreStats.ExplodedMines > 0 || _gameManager.HardcoreStats.IsGameOver)
                    {
                        SignalBus.Fire(
                            new GameOverSignal(
                                GameManager.Instance.CurrentGameMode,
                                _gameManager.HardcoreStats.IsGameOver,
                                _gameManager.HardcoreStats.IsGameWin
                            )
                        );
                    }
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

                    if (!TimeModeTimerManager.Instance.IsTimerRunning && TimeModeTimerManager.Instance.IsTimeUp())
                    {
                        SignalBus.Fire(
                            new GameOverSignal(
                                GameManager.Instance.CurrentGameMode,
                                _gameManager.TimeTrialStats.IsGameOver,
                                _gameManager.TimeTrialStats.IsGameWin
                            )
                        );
                        return;
                    }

                    if (_gameManager.TimeTrialStats.IsGameOver)
                    {
                        SignalBus.Fire(
                            new GameOverSignal(
                                GameManager.Instance.CurrentGameMode,
                                _gameManager.TimeTrialStats.IsGameOver,
                                _gameManager.TimeTrialStats.IsGameWin
                            )
                        );
                    }

                    else
                    {
                        TimeModeTimerManager.Instance.StartModeTimer();
                    }
                }
                else
                {
                    StartNewGame();
                    TimeModeTimerManager.Instance.StartModeTimer();
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
        _gameManager.CurrentStatisticController.IsGameStarted = true;
        _gameManager.CurrentStatisticController.StartTimer();        
    }

    private void SetCurrentRewardLevel() //MB и через свич
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
        if (IsInputLocked)
        {
            return;
        }

        float currentTime = Time.time;

        if (Input.GetMouseButtonDown(0))
        {
            // Проверка на двойной клик
            if (GameSettingsManager.Instance.OnDoubleClick)
            {
                if (currentTime - _lastClickTime <= DoubleClickThreshold)
                {
                    GetSectorAtDoubleClick();
                    _lastClickTime = 0; // Сброс времени клика
                    return;
                }
            }

            _clickStartTime = currentTime;
            _isHolding = true;
            _flagSet = false;
            _lastClickTime = currentTime; // Обновляем время последнего клика
        }


        if (Input.GetMouseButtonUp(0))
        {
            if (!_flagSet && _isHolding && Time.time - _clickStartTime < GameSettingsManager.Instance.HoldTime)
            {
                GetSectorAtClick();
            }
            _isHolding = false;

            if (!GameSettingsManager.Instance.OnDoubleClick)
            {
                GetSectorAtDoubleClick();
            }
        }

        if (_isHolding && !_flagSet && Time.time - _clickStartTime >= GameSettingsManager.Instance.HoldTime)
        {
            GetSectorForFlagAtClick();
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
        //int minesCount = CalculateProgressiveMinesCount();
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
        int absoluteMin = _minesConfig.AbsoluteMin; // 6
        int absoluteMax = _minesConfig.AbsoluteMax; // 37
        float difficultyStepCoef = _minesConfig.DifficultyStepCoef; // 1.25

        // Последовательность шагов (+1, +1, -1)
        int[] stepPattern = { 1, 1, -1 };
        int patternLength = stepPattern.Length;

        // Начинаем с абсолютного минимума
        int stepSum = 0;

        for (int i = 0; i < activeSectorCount - 1; i++)
        {
            stepSum += stepPattern[i % patternLength];
        }

        // Вычисляем границы мин
        int minMines = Mathf.Clamp(absoluteMin + stepSum, absoluteMin, absoluteMax);
        int maxMines = Mathf.Clamp((int)((absoluteMin + stepSum) * difficultyStepCoef), absoluteMin, absoluteMax);

        // Выбираем случайное количество мин в диапазоне
        int mines = Random.Range(minMines, maxMines + 1);

        //Debug.Log($"Sector: {activeSectorCount} | min: {minMines}, max: {maxMines} | mines: {mines}");
        return mines;
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

    private void GetSectorForFlagAtClick()
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
                if (!cell.IsExploded)
                {
                    CheckLoseConditions(cell.Sector, cell);     //check sector of this cell
                }                
                
                break;

            case CellState.Empty:
                StartCoroutine(Flood(currentSector, cell, _sectorsToRedraw));

                //CheckWinCondition();
                SectorCompletionCheck(cell.Sector);             //check sector of this cell
                break;

            default:
                cell.IsRevealed = true;

                UpdateOpenedCells();
                //SignalBus.Fire<CellRevealedSignal>();

                cell.IsActive = true;

                //CheckWinCondition();
                SectorCompletionCheck(cell.Sector);             //check sector of this cell                
                break;
        }

        _lastClickPosition = cell.GlobalCellPosition;
        _statisticController.SetLastClickPosition(_lastClickPosition);

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
        _statisticController.SetLastClickPosition(_lastClickPosition);

        // Вибрация и перерисовка
        if (GameSettingsManager.Instance.IsVibrationEnabled)
        {
            VibrateOnAction();
        }
        
        RedrawSectors();

        SectorCompletionCheck(cell.Sector);             //check sector of this cell
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
            UpdateOpenedCells();
            //SignalBus.Fire<CellRevealedSignal>();
        }

        affectedSectors.Add(currentSector);

        SectorCompletionCheck(cell.Sector);             //check sector of this cell

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

    private void CheckLoseConditions(Sector currentSector, InfiniteCell cell)
    {
        switch (GameManager.Instance.CurrentGameMode)
        {
            case GameMode.Hardcore:
                UpdateExplodedMinesCount();

                _statisticController.StopTimer();
                _statisticController.IsGameOver = true;
                cell.IsExploded = true;
                cell.IsRevealed = true;

                SaveCurrentGame();

                SignalBus.Fire(
                    new GameOverSignal(
                        GameMode.Hardcore,
                        GameManager.Instance.HardcoreStats.IsGameOver,
                        GameManager.Instance.HardcoreStats.IsGameWin
                    )
                );
                break;

            case GameMode.SimpleInfinite:

                UpdateExplodedMinesCount();
                UpdateSectorBuyoutLevel();

                currentSector.SetBuyoutCost(_sectorBuyoutCostConfig, _currentSectorBuyoutLevel);
                currentSector.ExplodeSector(cell);

                SaveCurrentGame();
                break;

            case GameMode.TimeTrial:

                if (!TimeModeTimerManager.Instance.IsTimerRunning && TimeModeTimerManager.Instance.IsTimeUp())
                {
                    SignalBus.Fire(
                        new GameOverSignal(
                            GameMode.Hardcore,
                            GameManager.Instance.TimeTrialStats.IsGameOver,
                            GameManager.Instance.TimeTrialStats.IsGameWin
                        )
                    );
                    return;
                }

                UpdateExplodedMinesCount();
                UpdateSectorBuyoutLevel();

                currentSector.SetBuyoutCost(_sectorBuyoutCostConfig, _currentSectorBuyoutLevel);
                currentSector.ExplodeSector(cell);

                SaveCurrentGame();
                break;
        }
        
    }

    private void RewardBonus(InfiniteCell cell)
    {
        if (!cell.IsAward) return;

        _statisticController.IncrementRewardLevel();
        SetCurrentRewardLevel();
        
        int currentReward = CalculateCurrentReward(_statisticController.RewardLevel);

        SignalBus.Fire(new OnGameRewardSignal(0, currentReward));
        //Debug.Log($"Номер награды: {_currentRewardLevel}, награда: {currentReward}");
        cell.IsAward = false;

        MoveAwardSprite(cell, _targetRewardUIElement);
    }

    private int CalculateCurrentReward(int collectedRewards)
    {
        int rewardLevelIndex = Mathf.Min(collectedRewards - 1, _sectorRewardConfig.RewardLevels.Count - 1);
        RewardLevel currentRewardLevel = _sectorRewardConfig.RewardLevels[rewardLevelIndex];
        
        float randomMultiplier = Random.Range(1f - currentRewardLevel.RandomizationCoefficient, 1f + currentRewardLevel.RandomizationCoefficient);
        int baseReward = Mathf.RoundToInt(currentRewardLevel.CurrencyAmount * randomMultiplier);
        
        baseReward = Mathf.Clamp(baseReward, currentRewardLevel.MinReward, currentRewardLevel.MaxReward);

        float modeMultiplier = _sectorRewardConfig.GetMultiplier(GameManager.Instance.CurrentGameMode);
        int finalReward = Mathf.FloorToInt(baseReward * modeMultiplier);

        return finalReward;
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
        
        _lastClickPosition = _statisticController.LastClickPosition;
        
        ReturnCameraToLastClick();
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
        //Debug.Log(_statisticController.SectorBuyoutCostLevel);
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
        _gameManager.CurrentStatisticController.StartTimer();

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
            //Debug.Log($"при загрузке: {newSector} position: {logicalPosition}");
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

    private void OnApplicationQuit()
    {
        _gameManager.CurrentStatisticController.StopTimer();
        _playerProgress.SavePlayerProgress();

        if (IsFirstClick)
        {
            SaveCurrentGame();
        }
    }
}
