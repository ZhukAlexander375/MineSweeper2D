using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-1)]
public class SimpleGridManager : MonoBehaviour
{
    [Header("Camera Controller")]
    [SerializeField] private CameraController _cameraController;
    //[SerializeField] private float _postCameraInteractionDelay = 0.15f; // Задержка после завершения работы камеры
      
    [Header("Settings")]
    [SerializeField] private Board _board;
    //[SerializeField] private List<LevelConfig> _levels = new();
    [SerializeField] private GameObject _flagPlaceParticle;
    [SerializeField] private GameObject _flagRemoveParticle;
    //[SerializeField] private FreeForm _freeForm;

    private int _width;
    private int _height;
    private int _mineCount;

    private CellGrid _cellGrid;
    private int _currentLevel = 0;

    public bool IsFirstClick;
    private bool IsGameOver;
    private bool IsGenerated;

    private float _clickStartTime;
    private bool _isHolding;
    private bool _flagSet;
    private float _lastClickTime = -1f;
    private const float DoubleClickThreshold = 0.3f; // Порог для двойного клика (в секундах)

    private SaveManager _saveManager;
    private PlayerProgress _playerProgress;
    private GameManager _gameManager;
    private IStatisticController _statisticController;

    //private float _lastCameraInteractionTime = 0f;
    private bool IsInputLocked;

    private void Awake()
    {

    }

    private void Start()
    {
        _saveManager = SaveManager.Instance;
        _playerProgress = PlayerProgress.Instance;
        _gameManager = GameManager.Instance;
        _statisticController = GameManager.Instance.CurrentStatisticController;

        CheckGameStart();
    }

    private void Update()
    {
        if (_cameraController.IsCameraInteracting)
        {
            IsInputLocked = true;
            _isHolding = false;
            return;
        }

        if (_cameraController.HasFinishedInteracting)
        { 
            _cameraController.ResetFinishedInteracting();
            IsInputLocked = false;
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

        IsInputLocked = false;

        if (!IsInputLocked)
        {
            HandleGameInput();
        }
    }

    private void HandleGameInput()
    {
        if (IsInputLocked)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            float currentTime = Time.time;

            // Проверка на двойной клик
            if (currentTime - _lastClickTime <= DoubleClickThreshold)
            {
                Unchord();
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
                Reveal();
            }
            _isHolding = false;
        }

        if (_isHolding && !_flagSet && Time.time - _clickStartTime >= GameSettingsManager.Instance.HoldTime)
        {
            Flag();
            _flagSet = true;
        }        
    }

    private void CheckGameStart()
    {
        switch (GameManager.Instance.CurrentGameMode)
        {
            case GameMode.ClassicEasy:
                if (_gameManager.ClassicStats.IsGameStarted)
                {
                    LoadSavedGame();
                    SignalBus.Fire<LoadCompletedSignal>();

                    if (_gameManager.ClassicStats.IsGameOver || _gameManager.ClassicStats.IsGameWin)
                    {
                        SignalBus.Fire(
                            new GameOverSignal(
                                GameManager.Instance.CurrentGameMode,
                                _gameManager.ClassicStats.IsGameOver,
                                _gameManager.ClassicStats.IsGameWin
                            )
                        );
                    }
                }
                else
                {
                    StartLevel(0);
                }                
                break;

            case GameMode.ClassicMedium:
                if (_gameManager.ClassicStats.IsGameStarted)
                {
                    LoadSavedGame();
                    if (_gameManager.ClassicStats.IsGameOver || _gameManager.ClassicStats.IsGameWin)
                    {
                        SignalBus.Fire(
                            new GameOverSignal(
                                GameManager.Instance.CurrentGameMode,
                                _gameManager.ClassicStats.IsGameOver,
                                _gameManager.ClassicStats.IsGameWin
                            )
                        );
                    }
                }
                else
                {
                    StartLevel(1);
                }
                break;

            case GameMode.ClassicHard:
                if (_gameManager.ClassicStats.IsGameStarted)
                {
                    LoadSavedGame();
                    if (_gameManager.ClassicStats.IsGameOver || _gameManager.ClassicStats.IsGameWin)
                    {
                        SignalBus.Fire(
                            new GameOverSignal(
                                GameManager.Instance.CurrentGameMode,
                                _gameManager.ClassicStats.IsGameOver,
                                _gameManager.ClassicStats.IsGameWin
                            )
                        );
                    }
                }
                else
                {
                    StartLevel(2);
                }
                break;

            case GameMode.Custom:
                if (_gameManager.ClassicStats.IsGameStarted)
                {
                    LoadSavedGame();
                    if (_gameManager.ClassicStats.IsGameOver || _gameManager.ClassicStats.IsGameWin)
                    {
                        SignalBus.Fire(
                            new GameOverSignal(
                                GameManager.Instance.CurrentGameMode,
                                _gameManager.ClassicStats.IsGameOver,
                                _gameManager.ClassicStats.IsGameWin
                            )
                        );
                    }
                }
                else
                {
                    StartCustomLevel(_gameManager.CustomLevel);
                }
                break;
        }
    }

    private void StartLevel(int levelIndex)
    {        
        var levels = GameManager.Instance.PredefinedLevels;

        if (levelIndex >= 0 && levelIndex < levels.Count)
        {
            _currentLevel = levelIndex;
            SetLevelSettings(levels[_currentLevel]);
            NewGame();
        }
        else
        {
            Debug.LogError("Level index out of range");
        }
    }

    private void StartCustomLevel(LevelConfig customLevel)
    {
        SetLevelSettings(customLevel);
        NewGame();
    }

    private void SetLevelSettings(LevelConfig level)
    {
        _width = level.Width;
        _height = level.Height;        
        _mineCount = Mathf.Clamp(level.MineCount, 0, _width * _height);

        AdjustCameraToGridSize();
        //Camera.main.transform.position = new Vector3(_width / 2f, _height / 2f, -10f);
    }
    private void NewGame()
    {
        _gameManager.ResetCurrentModeStatistic();
        StopAllCoroutines();

        _cellGrid = new CellGrid(_width, _height);
        _board.Draw(_cellGrid);

        _gameManager.CurrentStatisticController.IsGameStarted = true;
        _gameManager.CurrentStatisticController.StartTimer();

        //_cellGrid = new CellGrid((int)Mathf.Sqrt(_freeForm.GridSize), (int)Mathf.Sqrt(_freeForm.GridSize));
        //_board.DrawFreeForm(_freeForm, _cellGrid);
    }
        

    public void AdjustCameraToGridSize()
    {
        Camera camera = Camera.main;
        if (camera.orthographic)
        {
            float aspectRatio = (float)Screen.width / Screen.height;

            float cameraSizeX = _width / 2f;
            float cameraSizeY = _height / 2f;

            camera.orthographicSize = Mathf.Max(cameraSizeY, cameraSizeX / aspectRatio);

            camera.transform.position = new Vector3(_width / 2f, _height / 2f, -10f);
        }
        else
        {
            Debug.LogError("Camera is not set to orthographic mode.");
        }
    }

    private void Reveal()
    {
        if (!IsFirstClick)
        {
            IsFirstClick = true;
        }

        if (TryGetCellAtMousePosition(out BaseCell cell))
        {
            if (!IsGenerated)
            {
                _cellGrid.GenerateMines(cell, _mineCount);
                _cellGrid.GenerateNumbers();
                IsGenerated = true;
            }

            Reveal(cell);
        }
    }

    private void Reveal(BaseCell cell)
    {
        if (cell.IsRevealed) return;
        if (cell.IsFlagged) return;

        switch (cell.CellState)
        {
            case CellState.Mine:
                UpdateExplodedMinesCount();
                Explode(cell);
                SaveCurrentGame();
                break;

            case CellState.Empty:
                StartCoroutine(Flood(cell));
                CheckWinCondition();
                break;

            default:
                cell.IsRevealed = true;
                UpdateOpenedCells();
                CheckWinCondition();
                break;
        }

        _board.Draw(_cellGrid);
    }

    private IEnumerator Flood(BaseCell cell)
    {
        if (IsGameOver) yield break;
        if (cell.IsRevealed) yield break;
        if (cell.CellState == CellState.Mine) yield break;

        cell.IsRevealed = true;

        if (cell.IsRevealed)
        {
            UpdateOpenedCells();
        }

        _board.Draw(_cellGrid);

        yield return null;

        if (cell.CellState == CellState.Empty)
        {
            if (_cellGrid.TryGetCell(cell.GlobalCellPosition.x - 1, cell.GlobalCellPosition.y, out BaseCell left))
            {
                StartCoroutine(Flood(left));
            }
            if (_cellGrid.TryGetCell(cell.GlobalCellPosition.x + 1, cell.GlobalCellPosition.y, out BaseCell right))
            {
                StartCoroutine(Flood(right));
            }
            if (_cellGrid.TryGetCell(cell.GlobalCellPosition.x, cell.GlobalCellPosition.y - 1, out BaseCell down))
            {
                StartCoroutine(Flood(down));
            }
            if (_cellGrid.TryGetCell(cell.GlobalCellPosition.x, cell.GlobalCellPosition.y + 1, out BaseCell up))
            {
                StartCoroutine(Flood(up));
            }
            if (_cellGrid.TryGetCell(cell.GlobalCellPosition.x - 1, cell.GlobalCellPosition.y - 1, out BaseCell downLeft))
            {
                StartCoroutine(Flood(downLeft));
            }
            if (_cellGrid.TryGetCell(cell.GlobalCellPosition.x + 1, cell.GlobalCellPosition.y - 1, out BaseCell downRight))
            {
                StartCoroutine(Flood(downRight));
            }
            if (_cellGrid.TryGetCell(cell.GlobalCellPosition.x - 1, cell.GlobalCellPosition.y + 1, out BaseCell upLeft))
            {
                StartCoroutine(Flood(upLeft));
            }
            if (_cellGrid.TryGetCell(cell.GlobalCellPosition.x + 1, cell.GlobalCellPosition.y + 1, out BaseCell upRight))
            {
                StartCoroutine(Flood(upRight));
            }
        }
    }

    private void Flag()
    {
        if (!IsFirstClick)
        {
            IsFirstClick = true;
        }

        if (!TryGetCellAtMousePosition(out BaseCell cell)) return;
        if (cell.IsRevealed) return;

        bool isPlacingFlag = !cell.IsFlagged;
        cell.IsFlagged = !cell.IsFlagged;

        UpdateFlagsCount(isPlacingFlag);
        InstantiateParticleAtCell(isPlacingFlag ? _flagPlaceParticle : _flagRemoveParticle, cell);

        if (GameSettingsManager.Instance.IsVibrationEnabled)
        {
            VibrateOnAction();
        }

        _board.Draw(_cellGrid);
        CheckWinCondition();
    }

    private void InstantiateParticleAtCell(GameObject particlePrefab, BaseCell cell)
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

    private void Unchord()
    {
        // Сброс состояния всех ячеек
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _cellGrid[x, y].Chorded = false;
            }
        }

        if (TryGetCellAtMousePosition(out BaseCell clickedCell))
        {
            // Если ячейка не открыта и не помечена флагом, помечаем соседние ячейки
            if (!clickedCell.IsRevealed && !clickedCell.IsFlagged)
            {
                for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
                {
                    for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
                    {
                        int x = clickedCell.GlobalCellPosition.x + adjacentX;
                        int y = clickedCell.GlobalCellPosition.y + adjacentY;

                        if (_cellGrid.TryGetCell(x, y, out BaseCell adjacentCell))
                        {
                            adjacentCell.Chorded = !adjacentCell.IsRevealed && !adjacentCell.IsFlagged;
                        }
                    }
                }
            }
            else if (clickedCell.IsRevealed && clickedCell.CellState == CellState.Number)
            {
                // Проверяем флаги вокруг и открываем ячейки
                for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
                {
                    for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
                    {
                        if (adjacentX == 0 && adjacentY == 0) continue;

                        int x = clickedCell.GlobalCellPosition.x + adjacentX;
                        int y = clickedCell.GlobalCellPosition.y + adjacentY;

                        if (_cellGrid.TryGetCell(x, y, out BaseCell adjacentCell))
                        {
                            if (_cellGrid.CountAdjacentFlags(clickedCell) >= clickedCell.CellNumber)
                            {
                                Reveal(adjacentCell);
                            }
                        }
                    }
                }
            }
        }

        _board.Draw(_cellGrid);
    }

    private void Explode(BaseCell cell)
    {
        _statisticController.StopTimer();
        _statisticController.IsGameOver = true;
        _statisticController.IsGameWin = false;

        SignalBus.Fire(
            new GameOverSignal(
                GameManager.Instance.CurrentGameMode,
                _gameManager.ClassicStats.IsGameOver,
                _gameManager.ClassicStats.IsGameWin
            )
        );

        cell.IsExploded = true;
        cell.IsRevealed = true;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                cell = _cellGrid[x, y];

                if (cell.CellState == CellState.Mine)
                {
                    cell.IsRevealed = true;
                }
            }
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

    private void CheckWinCondition()
    {
        bool allNonMinesRevealed = true;
        bool allMinesFlagged = true;

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                BaseCell cell = _cellGrid[x, y];

                // All non-mine cells must be revealed to have won
                if (!cell.IsRevealed && cell.CellState != CellState.Mine)
                {
                    allNonMinesRevealed = false;
                }

                if (cell.CellState == CellState.Mine && !cell.IsFlagged)
                {
                    allMinesFlagged = false;
                }

                // Если хотя бы одно из условий не выполнено, игра не выиграна
                if (!allNonMinesRevealed || !allMinesFlagged)
                {
                    return;
                }
            }
        }

        HandleWinCondition();
    }

    private void HandleWinCondition()
    {
        _statisticController.StopTimer();
        _statisticController.IsGameOver = false;
        _statisticController.IsGameWin = true;

        SignalBus.Fire(
            new GameOverSignal(
                GameManager.Instance.CurrentGameMode,
                _gameManager.ClassicStats.IsGameOver,
                _gameManager.ClassicStats.IsGameWin
            )
        );
         
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                BaseCell cell = _cellGrid[x, y];

                if (cell.CellState == CellState.Mine)
                {
                    cell.IsFlagged = true;
                }
            }
        }

        _board.Draw(_cellGrid);
    }

    public void SaveCurrentGame()
    {
        switch (GameManager.Instance.CurrentGameMode)
        {
            case GameMode.ClassicEasy:
            case GameMode.ClassicMedium:
            case GameMode.ClassicHard:
            case GameMode.Custom:
                _saveManager.SaveClassicGame(_cellGrid, GameManager.Instance.ClassicStats);
                break;

            default:
                Debug.LogError("Unknown game mode. Cannot save.");
                break;
        }
    }

    private void LoadSavedGame()
    {
        var (gridData, classicModeData) = _saveManager.LoadClassicGame();

        if (gridData == null)
        {
            //Debug.Log("No saved data found. Starting a new game.");
            switch (GameManager.Instance.CurrentGameMode)
            {
                case GameMode.ClassicEasy:
                    StartLevel(0);
                    break;

                case GameMode.ClassicMedium:
                    StartLevel(1);
                    break;

                case GameMode.ClassicHard:
                    StartLevel(2);
                    break;

                case GameMode.Custom:
                    StartCustomLevel(_gameManager.CustomLevel);
                    break;
            }
            return;
        }

        _width = gridData.Width;
        _height = gridData.Height;

        _cellGrid = new CellGrid(_width, _height);
        _cellGrid.InitialializeCellsFromData(gridData.Cells);
        _board.Draw(_cellGrid);

        AdjustCameraToGridSize();
        _gameManager.CurrentStatisticController.StartTimer();

        IsFirstClick = true;
    }

    private bool TryGetCellAtMousePosition(out BaseCell cell)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = _board.Tilemap.WorldToCell(worldPosition);
        return _cellGrid.TryGetCell(cellPosition.x, cellPosition.y, out cell);
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
