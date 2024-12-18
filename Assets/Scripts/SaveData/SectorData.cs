using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SectorData
{
    public Vector2Int SectorPosition;
    public bool IsActive;
    public bool IsFirstCellActivated;
    public bool IsExploded;
    public bool IsPrizePlaced;
    public List<CellData> Cells = new List<CellData>();
    public bool IsLOADED;
    public bool IsCellsInitialized;
    public bool IsSectorCompleted;
    public int CurrentBuyoutCost;
}
