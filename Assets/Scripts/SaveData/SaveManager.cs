using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SaveInfinateGameFileName = "InfiniteGameSave.json";
    private const string SaveThemeFileName = "Theme.json";
    private const string SaveSettingsFileName = "GameSettings.json";
    private const string SavePlayerProgressFileName = "PlayerProgress.json";
    private const string SaveGameModesFileName = "GameModes.json";

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

    public void SaveInfiniteGame(List<SectorData> sectors)
    {
        InfiniteGameSaveWrapper saveData = new InfiniteGameSaveWrapper { Sectors = sectors };
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
        InfiniteGameSaveWrapper saveData = JsonUtility.FromJson<InfiniteGameSaveWrapper>(json);
        return saveData.Sectors;
    }

    public void ClearSavedInfiniteGame()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveInfinateGameFileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"Save file at {filePath} has been deleted.");
        }
        else
        {
            Debug.LogWarning("No save file found to delete.");
        }
    }

    public void SaveSelectedTheme(int selectedThemeIndex)
    {
        ThemeSaveWrapper themeData = new ThemeSaveWrapper { SelectedThemeIndex = selectedThemeIndex };
        string json = JsonUtility.ToJson(themeData, true);
        var filePath = Path.Combine(Application.persistentDataPath, SaveThemeFileName);
        File.WriteAllText(filePath, json);
        //Debug.Log($"Theme saved successfully to {filePath}");
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
        ThemeSaveWrapper themeData = JsonUtility.FromJson<ThemeSaveWrapper>(json);
        return themeData.SelectedThemeIndex;
    }

    public void SaveSettings(SettingsData settings)
    {
        SettingsWrapper wrapper = new SettingsWrapper();
        wrapper.SettingsData = settings;

        string json = JsonUtility.ToJson(wrapper, true);
        var filePath = Path.Combine(Application.persistentDataPath, SaveSettingsFileName);
        File.WriteAllText(filePath, json);
    }

    public SettingsData LoadSettings()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveSettingsFileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Settings save file not found.");
            return new SettingsData();
        }

        string json = File.ReadAllText(filePath);
        SettingsWrapper wrapper = JsonUtility.FromJson<SettingsWrapper>(json);
        return wrapper.SettingsData;
    }

    public void SavePlayerProgress(PlayerProgressData playerData)
    {
        PlayerProgressWrapper wrapper = new PlayerProgressWrapper();
        wrapper.PlayerData = playerData;

        string json = JsonUtility.ToJson(wrapper, true);
        var filePath = Path.Combine(Application.persistentDataPath, SavePlayerProgressFileName);
        File.WriteAllText(filePath, json);
    }

    public PlayerProgressData LoadPlayerProgress()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SavePlayerProgressFileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Player progress save file not found.");
            return new PlayerProgressData();
        }

        string json = File.ReadAllText(filePath);
        PlayerProgressWrapper wrapper = JsonUtility.FromJson<PlayerProgressWrapper>(json);
        return wrapper.PlayerData;
    }

    public void SaveGameModes(GameModesData gameModeData)
    {
        GameModesWrapper gameModesWrapper = new GameModesWrapper();
        gameModesWrapper.GameModesData = gameModeData;

        string json = JsonUtility.ToJson(gameModesWrapper, true);
        var filePath = Path.Combine(Application.persistentDataPath, SaveGameModesFileName);
        File.WriteAllText(filePath, json);
    }

    public GameModesData LoadGameModes()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveGameModesFileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Game modes save file not found.");
            return new GameModesData();
        }
        string json = File.ReadAllText(filePath);
        GameModesWrapper wrapper = JsonUtility.FromJson<GameModesWrapper>(json);
        return wrapper.GameModesData;
    }
}
