
using UnityEngine;

[CreateAssetMenu(fileName = "Mines Config", menuName = "Create Mines Config")]


public class MinesConfig : ScriptableObject
{
    public int MinMinesCount;
    public int MaxMinesCount;
    public int AbsoluteMax;
    public int AbsoluteMin;
    public int DifficultyStepCoef;
    public int IncrementMaxCoef;
}
