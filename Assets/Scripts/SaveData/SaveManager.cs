using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string SaveFileName = "GameSave.json";

    public bool HasSavedData()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        return File.Exists(filePath);
    }

    public List<SectorData> LoadSavedSectors()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("No saved data found.");
            return new List<SectorData>();
        }

        var json = File.ReadAllText(filePath);
        var saveData = JsonUtility.FromJson<SaveDataWrapper>(json);
        return saveData.Sectors;
    }
}
