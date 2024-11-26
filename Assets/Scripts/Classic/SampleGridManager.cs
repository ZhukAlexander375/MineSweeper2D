using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[DefaultExecutionOrder(-1)]
public class SampleGridManager : MonoBehaviour
{
    [SerializeField] private Board _board;
    [SerializeField] private List<LevelConfig> _levels = new();
    [SerializeField] private GameObject _flagPlaceParticle;
    [SerializeField] private GameObject _flagRemoveParticle;
    //[SerializeField] private FreeForm _freeForm;

    private int _width;
    private int _height;
    private int _mineCount;
        
    private CellGrid _cellGrid;
    private int _currentLevel = 0;

    private bool IsGameOver;
    private bool IsGenerated;

    private float _clickStartTime;
    private bool _isHolding;
    private bool _flagSet;
    private float _lastClickTime = -1f;
    private const float DoubleClickThreshold = 0.3f; // Порог для двойного клика (в секундах)

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        gameObject.SetActive(false);
        //NewGame();
    }

    public void StartLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < _levels.Count)
        {
            _currentLevel = levelIndex;
            SetLevelSettings();
            NewGame();
        }
        else
        {
            Debug.LogError("Level index out of range");
        }
    }

    public void StartCustomLevel(ClassicGameSettings settings)
    {
        SetCustomLevelSettings(settings);
        NewGame();       
    }

    private void NewGame()
    {
        StopAllCoroutines();               

        IsGameOver = false;
        IsGenerated = false;

        _cellGrid = new CellGrid(_width, _height);
        _board.Draw(_cellGrid);

        //_cellGrid = new CellGrid((int)Mathf.Sqrt(_freeForm.GridSize), (int)Mathf.Sqrt(_freeForm.GridSize));
        //_board.DrawFreeForm(_freeForm, _cellGrid);
    }

    private void SetLevelSettings()
    {
        _width = _levels[_currentLevel].Width;
        _height = _levels[_currentLevel].Height;
        _mineCount = _levels[_currentLevel].MineCount;
        _mineCount = Mathf.Clamp(_mineCount, 0, _width * _height);

        AdjustCameraToGridSize(_width, _height);
        //Camera.main.transform.position = new Vector3(_width / 2f, _height / 2f, -10f);
    }

    private void SetCustomLevelSettings(ClassicGameSettings settings)
    {
        int width = ClassicGameMinSize.ClampValue(settings.Width, ClassicGameMinSize.MinWidth, ClassicGameMinSize.MaxWidth);
        int height = ClassicGameMinSize.ClampValue(settings.Height, ClassicGameMinSize.MinHeight, ClassicGameMinSize.MaxHeight);
        int maxMines = ClassicGameMinSize.MaxMines(width, height);
        int mines = ClassicGameMinSize.ClampValue(settings.Mines, ClassicGameMinSize.MinMines(width, height), maxMines);

        _width = width;
        _height = height;
        _mineCount = mines;

        AdjustCameraToGridSize(width, height);
        //Camera.main.transform.position = new Vector3(_width / 2f, _height / 2f, -10f);
    }

    private void AdjustCameraToGridSize(int gridWidth, int gridHeight)
    {        
        Camera camera = Camera.main;
        if (camera.orthographic)
        {
            float aspectRatio = (float)Screen.width / Screen.height;
            
            float cameraSizeX = gridWidth / 2f;
            float cameraSizeY = gridHeight / 2f;

            camera.orthographicSize = Mathf.Max(cameraSizeY, cameraSizeX / aspectRatio);

            camera.transform.position = new Vector3(gridWidth / 2f, gridHeight / 2f, -10f);
        }
        else
        {
            Debug.LogError("Camera is not set to orthographic mode.");
        }
    }

    private void Update()
    {
        // check click on ui or gamefield
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
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
            if (!_flagSet && _isHolding && Time.time - _clickStartTime < 0.3f)
            {
                Reveal();
            }
            _isHolding = false;
        }

        if (_isHolding && !_flagSet && Time.time - _clickStartTime >= 0.3f)
        {
            Flag();
            _flagSet = true;
        }
        /*if (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            NewGame();
            return;
        }

        if (!IsGameOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Reveal();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Flag();
            }
            else if (Input.GetMouseButton(2))
            {
                Chord();
            }
            else if (Input.GetMouseButtonUp(2))
            {
                Unchord();
            }
        }*/
    }

    private void Reveal()
    {
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
                Explode(cell);
                break;

            case CellState.Empty:
                StartCoroutine(Flood(cell));
                CheckWinCondition();
                break;

            default:
                cell.IsRevealed = true;
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
        _board.Draw(_cellGrid);

        yield return null;

        if (cell.CellState == CellState.Empty)
        {
            if (_cellGrid.TryGetCell(cell.CellPosition.x - 1, cell.CellPosition.y, out BaseCell left))
            {
                StartCoroutine(Flood(left));
            }
            if (_cellGrid.TryGetCell(cell.CellPosition.x + 1, cell.CellPosition.y, out BaseCell right))
            {
                StartCoroutine(Flood(right));
            }
            if (_cellGrid.TryGetCell(cell.CellPosition.x, cell.CellPosition.y - 1, out BaseCell down))
            {
                StartCoroutine(Flood(down));
            }
            if (_cellGrid.TryGetCell(cell.CellPosition.x, cell.CellPosition.y + 1, out BaseCell up))
            {
                StartCoroutine(Flood(up));
            }
            if (_cellGrid.TryGetCell(cell.CellPosition.x - 1, cell.CellPosition.y - 1, out BaseCell downLeft))
            {
                StartCoroutine(Flood(downLeft));
            }
            if (_cellGrid.TryGetCell(cell.CellPosition.x + 1, cell.CellPosition.y - 1, out BaseCell downRight))
            {
                StartCoroutine(Flood(downRight));
            }
            if (_cellGrid.TryGetCell(cell.CellPosition.x - 1, cell.CellPosition.y + 1, out BaseCell upLeft))
            {
                StartCoroutine(Flood(upLeft));
            }
            if (_cellGrid.TryGetCell(cell.CellPosition.x + 1, cell.CellPosition.y + 1, out BaseCell upRight))
            {
                StartCoroutine(Flood(upRight));
            }
        }
    }

    private void Flag()
    {
        if (!TryGetCellAtMousePosition(out BaseCell cell)) return;
        if (cell.IsRevealed) return;

        bool isPlacingFlag = !cell.IsFlagged;
        cell.IsFlagged = !cell.IsFlagged;

        InstantiateParticleAtCell(isPlacingFlag ? _flagPlaceParticle : _flagRemoveParticle, cell);
        VibrateOnAction();

        _board.Draw(_cellGrid);
    }

    private void InstantiateParticleAtCell(GameObject particlePrefab, BaseCell cell)
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
                        int x = clickedCell.CellPosition.x + adjacentX;
                        int y = clickedCell.CellPosition.y + adjacentY;

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

                        int x = clickedCell.CellPosition.x + adjacentX;
                        int y = clickedCell.CellPosition.y + adjacentY;

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
        IsGameOver = true;

        // Set the mine as exploded
        cell.IsExploded = true;
        cell.IsRevealed = true;

        // Reveal all other mines
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

    private void CheckWinCondition()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                BaseCell cell = _cellGrid[x, y];

                // All non-mine cells must be revealed to have won
                if (cell.CellState != CellState.Mine && !cell.IsRevealed)
                {
                    return; // no win
                }
            }
        }

        IsGameOver = true;

        // Flag all the mines
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
    }

    private bool TryGetCellAtMousePosition(out BaseCell cell)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = _board.Tilemap.WorldToCell(worldPosition);
        return _cellGrid.TryGetCell(cellPosition.x, cellPosition.y, out cell);
    }
}
