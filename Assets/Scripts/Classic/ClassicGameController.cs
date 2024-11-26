using UnityEngine;

public class ClassicGameController : MonoBehaviour
{
    [SerializeField] private SampleGridManager _gridManager;

    public void StartPresetLevel(int levelIndex)
    {
        _gridManager.StartLevel(levelIndex);
    }

    public void StartCustomLevel(ClassicGameSettings settings)
    {
        _gridManager.StartCustomLevel(settings);
    }
}
