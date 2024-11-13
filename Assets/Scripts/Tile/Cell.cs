using UnityEngine;

public class Cell
{
    public CellState CellState;
    public Vector3Int CellPosition;
    public int CellNumber;
    public bool IsFlagged;
    public bool IsExploded;
    public bool Сhorded;
    public bool IsActive;

    private bool _isRevealed;
    private Sector _sector;
    private GridManager _gridManager;

    public bool IsRevealed
    {
        get => _isRevealed;
        set
        {
            if (!_isRevealed && value)  // Если значение изменяется с false на true
            {
                _isRevealed = true;
                CellReveal();
                SetActiveAdjacentCells();
            }
            else
            {
                _isRevealed = value;
            }
        }
    }
   
    public void SetOwnerSector(Sector sector)
    {
        _sector = sector;
    }

    public void SetGridManager(GridManager gridManager)
    {
        _gridManager = gridManager;
    }

    public void CellReveal()
    {
        _sector.GenerateNumbersInSector();
    }

    public void SetActiveAdjacentCells()
    {
        if (IsRevealed && CellState != CellState.Mine)
        {
            _gridManager.CellsActivate(this);
        }       
    }
}
