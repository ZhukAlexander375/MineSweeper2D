
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FreeForm", menuName = "Create Free Form")]
public class FreeForm : ScriptableObject
{
    public int GridSize;
    public int MineCount;
    public List<bool> ActiveCells;
}
