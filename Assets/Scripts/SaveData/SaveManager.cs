using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SaveInfinateGameFileName = "InfiniteGameSave.json";
    private const string SaveThemeFileName = "Theme.json";

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
        var filePath = Path.Combine(Application.persistentDataPath, SaveInfinateGameFileName);
        return File.Exists(filePath);
    }

    public void SaveGame(List<SectorData> sectors)
    {
        SaveDataWrapper saveData = new SaveDataWrapper { Sectors = sectors };
        string json = JsonUtility.ToJson(saveData, true);
        var filePath = Path.Combine(Application.persistentDataPath, SaveInfinateGameFileName);
        File.WriteAllText(filePath, json);
        Debug.Log($"Game saved successfully to {filePath}");
    }

    public List<SectorData> LoadSavedSectors()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveInfinateGameFileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Save file not found.");
            return new List<SectorData>();
        }

        string json = File.ReadAllText(filePath);
        SaveDataWrapper saveData = JsonUtility.FromJson<SaveDataWrapper>(json);
        return saveData.Sectors;
    }

    public void SaveSelectedTheme(int selectedThemeIndex)
    {
        SaveDataWrapper themeData = new SaveDataWrapper { SelectedThemeIndex = selectedThemeIndex };
        string json = JsonUtility.ToJson(themeData, true);
        var filePath = Path.Combine(Application.persistentDataPath, SaveThemeFileName);
        File.WriteAllText(filePath, json);
        Debug.Log($"Theme saved successfully to {filePath}");
    }

    public int LoadSelectedTheme()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveThemeFileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Theme save file not found.");
            return 0;
        }

        string json = File.ReadAllText(filePath);
        SaveDataWrapper themeData = JsonUtility.FromJson<SaveDataWrapper>(json);
        return themeData.SelectedThemeIndex;
    }
}
