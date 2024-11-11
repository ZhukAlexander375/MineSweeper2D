using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Create Level")]
public class LevelConfig : ScriptableObject
{
    public int Width;
    public int Height;
    public int MineCount;    
}
