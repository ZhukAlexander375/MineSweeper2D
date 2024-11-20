using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class SampleGridManager : MonoBehaviour
{
    [SerializeField] private Board _board;
    [SerializeField] private List<LevelConfig> _levels = new();
    //[SerializeField] private FreeForm _freeForm;

    private int _width;
    private int _height;
    private int _mineCount;
        
    private CellGrid _cellGrid;
    private int _currentLevel = 0;

    private bool IsGameOver;
    private bool IsGenerated;

    private void Awake()
    {
        Application.targetFrameRate = 60;        
    }

    private void Start()
    {
        NewGame();
    }

    private void NewGame()
    {
        StopAllCoroutines();

        SetLevelSettings();
        

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
        Camera.main.transform.position = new Vector3(_width / 2f, _height / 2f, -10f);
    }

    /*private void OnValidate()
    {
        _mineCount = Mathf.Clamp(_mineCount, 0, _width * _height);
    }*/

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
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
        }
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
        }
    }

    private void Flag()
    {
        if (!TryGetCellAtMousePosition(out BaseCell cell)) return;
        if (cell.IsRevealed) return;

        cell.IsFlagged = !cell.IsFlagged;
        _board.Draw(_cellGrid);
    }

    private void Chord()
    {
        // unchord previous cells
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _cellGrid[x, y].Chorded = false;
            }
        }

        // chord new cells
        if (TryGetCellAtMousePosition(out BaseCell chord))
        {
            for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
            {
                for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
                {
                    int x = chord.CellPosition.x + adjacentX;
                    int y = chord.CellPosition.y + adjacentY;

                    if (_cellGrid.TryGetCell(x, y, out BaseCell cell))
                    {
                        cell.Chorded = !cell.IsRevealed && !cell.IsFlagged;
                    }
                }
            }
        }

        _board.Draw(_cellGrid);
    }

    private void Unchord()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                BaseCell cell = _cellGrid[x, y];

                if (cell.Chorded)
                {
                    Unchord(cell);
                }
            }
        }

        _board.Draw(_cellGrid);
    }

    private void Unchord(BaseCell chord)
    {
        chord.Chorded = false;

        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0)
                {
                    continue;
                }

                int x = chord.CellPosition.x + adjacentX;
                int y = chord.CellPosition.y + adjacentY;

                if (_cellGrid.TryGetCell(x, y, out BaseCell cell))
                {
                    if (cell.IsRevealed && cell.CellState == CellState.Number)
                    {
                        if (_cellGrid.CountAdjacentFlags(cell) >= cell.CellNumber)
                        {
                            Reveal(chord);
                            return;
                        }
                    }
                }
            }
        }
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
