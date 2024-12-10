using UnityEngine;

public class InfiniteCell : BaseCell
{
    public bool IsActive;
    public bool IsAward;
    public Sector Sector => _sector;

    private bool _isRevealed;
    private Sector _sector;
    private InfiniteGridManager _infiniteGridManager;

    public new bool IsRevealed
    {
        get => _isRevealed;
        set
        {
            if (_sector.IsLOADED)
            {
                _isRevealed = value;
                return;
            }

            if (!_isRevealed && value)  // Если значение изменяется с false на true
            {
                _isRevealed = true;
                CellReveal();
                SetActiveAdjacentCells();
                SignalBus.Fire(new OnCellActiveSignal(this));
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

    public void SetGridManager(InfiniteGridManager gridManager)
    {
        _infiniteGridManager = gridManager;
    }

    public void CellReveal()
    {
        if (_sector == null)
        {
            Debug.LogError($"_sector is null for cell at position {GlobalCellPosition}.");
            return;
        }
        _sector.GenerateNumbersInSector();        
    }

    public void SetActiveAdjacentCells()
    {
        if (IsRevealed && CellState != CellState.Mine)
        {
            _infiniteGridManager.CellsActivate(this);
        }       
    }
}
