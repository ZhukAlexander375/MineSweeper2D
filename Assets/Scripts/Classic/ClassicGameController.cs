using UnityEngine;

public class ClassicGameController : MonoBehaviour
{
    [SerializeField] private SimpleGridManager _gridManager;

    public void StartPresetLevel(int levelIndex)
    {
        _gridManager.StartLevel(levelIndex);
    }

    public void StartCustomLevel(ClassicGameSettings settings)
    {
        _gridManager.StartCustomLevel(settings);
    }
}
