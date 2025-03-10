using UnityEngine;

[System.Serializable]
public class CellData
{
    public Vector3Int GlobalCellPosition;
    public CellState CellState;
    public bool IsActive;
    public bool IsAward;
    public bool IsRevealed;
    public bool IsFlagged;
    public bool IsExploded;
    public bool Chorded;
    public int CellNumber;
    public bool HasAnimated;
}
