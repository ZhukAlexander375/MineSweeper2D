using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SaveFileName = "GameSave.json";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool HasSavedData()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        return File.Exists(filePath);
    }

    public void SaveGame(List<SectorData> sectors)
    {
        SaveDataWrapper saveData = new SaveDataWrapper { Sectors = sectors };
        string json = JsonUtility.ToJson(saveData, true);
        var filePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        File.WriteAllText(filePath, json);
        Debug.Log($"Game saved successfully to {filePath}");
    }

    public List<SectorData> LoadSavedSectors()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file not found.");
            return new List<SectorData>();
        }

        string json = File.ReadAllText(filePath);
        SaveDataWrapper saveData = JsonUtility.FromJson<SaveDataWrapper>(json);
        return saveData.Sectors;
    }
}
