using UnityEngine;

public class BaseCell 
{    
    public Vector3Int GlobalCellPosition;
    public CellState CellState;
    public int CellNumber;
    public bool IsRevealed;
    public bool IsFlagged;
    public bool IsExploded;
    public bool Chorded;    
}
