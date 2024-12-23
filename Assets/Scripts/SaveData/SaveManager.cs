using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
        
    private const string SaveThemeFileName = "Theme.json";
    private const string SaveSettingsFileName = "GameSettings.json";
    private const string SavePlayerProgressFileName = "PlayerProgress.json";

    private const string SaveGameMetaDataFileName = "GameMetaData.json";        //  акой режим последний и состо€ние режимов (нью/загрузка)

    private const string SaveSimpleInfiniteGameFileName = "SimpleInfiniteGameSave.json";
    private const string SaveHardcoreGameFileName = "HardcoreInfiniteGameSave.json";
    private const string SaveTimeTrialGameFileName = "TimeTrialInfiniteGameSave.json";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        //LoadModesStats();
    }

    private void LoadModesStats()
    {        
        //var simpleStats = LoadSimpleInfiniteModeStats();
        //var hardcoreStats = LoadHardcoreModeStats();
        //var timeTrialStats = LoadTimeTrialModeStats();

        //GameManager.Instance.SetGameModeData(GameMode.SimpleInfinite, simpleStats);
        //GameManager.Instance.SetGameModeData(GameMode.Hardcore, hardcoreStats);
        //GameManager.Instance.SetGameModeData(GameMode.TimeTrial, timeTrialStats);
    }

    public bool HasSavedData(GameMode mode)
    {
        string filePath = GetSaveFilePath(mode);
        return File.Exists(filePath);
    }

    private string GetSaveFilePath(GameMode mode)
    {
        string fileName = mode switch
        {
            GameMode.SimpleInfinite => SaveSimpleInfiniteGameFileName,
            GameMode.Hardcore => SaveHardcoreGameFileName,
            GameMode.TimeTrial => SaveTimeTrialGameFileName,
            _ => throw new ArgumentOutOfRangeException()
        };

        return Path.Combine(Application.persistentDataPath, fileName);
    }

    public void SaveSimpleInfiniteGame(List<SectorData> sectors, SimpleInfiniteStatisticController simpleInfiniteStats)
    {
        SimpleInfiniteGameSaveWrapper saveData = new SimpleInfiniteGameSaveWrapper
        {
            Sectors = sectors,
            SimpleInfiniteModeData = new SimpleInfiniteModeData(simpleInfiniteStats)
            {
                IsGameStarted = simpleInfiniteStats.IsGameStarted,
                OpenedCells = simpleInfiniteStats.OpenedCells,
                PlacedFlags = simpleInfiniteStats.PlacedFlags,
                CompletedSectors = simpleInfiniteStats.CompletedSectors,
                ExplodedMines = simpleInfiniteStats.ExplodedMines,
                RewardLevel = simpleInfiniteStats.RewardLevel,
                SectorBuyoutCostLevel = simpleInfiniteStats.SectorBuyoutCostLevel,
                TotalPlayTime = simpleInfiniteStats.TotalPlayTime
    }
        };
        string json = JsonUtility.ToJson(saveData, true);
        var filePath = Path.Combine(Application.persistentDataPath, SaveSimpleInfiniteGameFileName);
        File.WriteAllText(filePath, json);
        //Debug.Log("сохранено в файл");
    }

    public void SaveHardcoreGame(List<SectorData> sectors, HardcoreStatisticController hardcoreStats)
    {
        HardcoreGameSaveWrapper saveData = new HardcoreGameSaveWrapper 
        { 
            Sectors = sectors,
            HardcoreModeData = new HardcoreModeData(hardcoreStats)
            {
                IsGameStarted = hardcoreStats.IsGameStarted,
                OpenedCells = hardcoreStats.OpenedCells,
                PlacedFlags = hardcoreStats.PlacedFlags,
                CompletedSectors = hardcoreStats.CompletedSectors,
                ExplodedMines = hardcoreStats.ExplodedMines,
                RewardLevel = hardcoreStats.RewardLevel,
                SectorBuyoutCostLevel = hardcoreStats.SectorBuyoutCostLevel,
                TotalPlayTime = hardcoreStats.TotalPlayTime
            }
        };
        string json = JsonUtility.ToJson(saveData, true);
        var filePath = Path.Combine(Application.persistentDataPath, SaveHardcoreGameFileName);
        File.WriteAllText(filePath, json);
        Debug.Log($"Hardcore Game saved successfully to {filePath}");
    }

    public void SaveTimeTrialGame(List<SectorData> sectors, TimeTrialStatisticController timeTrialStats)
    {
        TimeTrialGameSaveWrapper saveData = new TimeTrialGameSaveWrapper 
        {
            Sectors = sectors,
            TimeTrialModeData = new TimeTrialModeData(timeTrialStats)
            {
                IsGameStarted = timeTrialStats.IsGameStarted,
                OpenedCells = timeTrialStats.OpenedCells,
                PlacedFlags = timeTrialStats.PlacedFlags,
                CompletedSectors = timeTrialStats.CompletedSectors,
                ExplodedMines = timeTrialStats.ExplodedMines,
                RewardLevel = timeTrialStats.RewardLevel,
                SectorBuyoutCostLevel = timeTrialStats.SectorBuyoutCostLevel,
                TotalPlayTime = timeTrialStats.TotalPlayTime
            }
        };
        string json = JsonUtility.ToJson(saveData, true);
        var filePath = Path.Combine(Application.persistentDataPath, SaveTimeTrialGameFileName);
        File.WriteAllText(filePath, json);
        Debug.Log($"Time Trial Game saved successfully to {filePath}");
    }

    /// <summary>
    /// FOR LOAD ONLY MODES STATISTICS 
    /// </summary>
    
    public SimpleInfiniteModeData LoadSimpleInfiniteModeStats()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveSimpleInfiniteGameFileName);

        if (!File.Exists(filePath))
        {
            return new SimpleInfiniteModeData();           
        }

        string json = File.ReadAllText(filePath);
        SimpleInfiniteGameSaveWrapper saveData = JsonUtility.FromJson<SimpleInfiniteGameSaveWrapper>(json);

        Debug.Log($"загружено из {filePath}");
        return saveData.SimpleInfiniteModeData;
    }

    public HardcoreModeData LoadHardcoreModeStats()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveHardcoreGameFileName);

        if (!File.Exists(filePath))
        {
            return new HardcoreModeData();
        }

        string json = File.ReadAllText(filePath);
        HardcoreGameSaveWrapper saveData = JsonUtility.FromJson<HardcoreGameSaveWrapper>(json);
        
        Debug.Log($"Hardcore Game stats loaded successfully from {saveData.HardcoreModeData.OpenedCells}");
        return saveData.HardcoreModeData;
    }

    public TimeTrialModeData LoadTimeTrialModeStats()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveTimeTrialGameFileName);

        if (!File.Exists(filePath))
        {
            return new TimeTrialModeData();
        }

        string json = File.ReadAllText(filePath);
        TimeTrialGameSaveWrapper saveData = JsonUtility.FromJson<TimeTrialGameSaveWrapper>(json);

        Debug.Log($"Simple Infinite Game stats loaded successfully from {filePath}");
        return saveData.TimeTrialModeData;
    }



    /// <summary>
    /// FOR LOAD ONLY MODES GRIDS 
    /// </summary>

    public List<SectorData> LoadSimpleInfiniteGameGrid()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveSimpleInfiniteGameFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"No save file found for Simple Infinite Game at {filePath}");
            return null;
        }

        string json = File.ReadAllText(filePath);
        SimpleInfiniteGameSaveWrapper saveData = JsonUtility.FromJson<SimpleInfiniteGameSaveWrapper>(json);

        Debug.Log($"Simple Infinite Game sectors loaded successfully from {filePath}");
        return saveData.Sectors;
    }

    public List<SectorData> LoadHardcoreGameGrid()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveHardcoreGameFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"No save file found for Simple Infinite Game at {filePath}");
            return null;
        }

        string json = File.ReadAllText(filePath);
        HardcoreGameSaveWrapper saveData = JsonUtility.FromJson<HardcoreGameSaveWrapper>(json);

        Debug.Log($"Simple Infinite Game sectors loaded successfully from {filePath}");
        return saveData.Sectors;
    }

    public List<SectorData> LoadTimeTrialGameGrid()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveTimeTrialGameFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"No save file found for Simple Infinite Game at {filePath}");
            return null;
        }

        string json = File.ReadAllText(filePath);
        TimeTrialGameSaveWrapper saveData = JsonUtility.FromJson<TimeTrialGameSaveWrapper>(json);

        Debug.Log($"Simple Infinite Game sectors loaded successfully from {filePath}");
        return saveData.Sectors;
    }

    /// <summary>
    /// FOR LOAD ALL MODES SAVES 
    /// </summary>
    /// 
    public (List<SectorData>, SimpleInfiniteModeData) LoadSimpleInfiniteGame()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveSimpleInfiniteGameFileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"No save file found for Simple Infinite Game at {filePath}");
            return (null, null);
        }

        string json = File.ReadAllText(filePath);
        SimpleInfiniteGameSaveWrapper saveData = JsonUtility.FromJson<SimpleInfiniteGameSaveWrapper>(json);

        Debug.Log($"Simple Infinite Game loaded successfully from {filePath}");
        return (saveData.Sectors, saveData.SimpleInfiniteModeData);
    }

    public (List<SectorData>, HardcoreModeData) LoadHardcoreGame()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveHardcoreGameFileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"Save file for Hardcore game not found at {filePath}");
            return (null, null);
        }

        string json = File.ReadAllText(filePath);
        HardcoreGameSaveWrapper saveData = JsonUtility.FromJson<HardcoreGameSaveWrapper>(json);
        return (saveData.Sectors, saveData.HardcoreModeData);
    }


    public (List<SectorData>, TimeTrialModeData) LoadTimeTrialGame()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveTimeTrialGameFileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"Save file for TimeTrial game not found at {filePath}");
            return (null, null);
        }

        string json = File.ReadAllText(filePath);
        TimeTrialGameSaveWrapper saveData = JsonUtility.FromJson<TimeTrialGameSaveWrapper>(json);
        return (saveData.Sectors, saveData.TimeTrialModeData);
    }

    public void ClearSavedSimpleInfiniteGame()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveSimpleInfiniteGameFileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            //Debug.Log($"Save file at {filePath} has been deleted.");
        }
        else
        {
            //Debug.LogWarning("No save file found to delete for SimpleInfinite.");
        }
    }

    public void ClearSavedHardcoreGame()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveHardcoreGameFileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            //Debug.Log($"Save file at {filePath} has been deleted.");
        }
        else
        {
            //Debug.LogWarning("No save file found to delete for Hardcore.");
        }
    }

    public void ClearSavedTimeTrialGame()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveTimeTrialGameFileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            //Debug.Log($"Save file at {filePath} has been deleted.");
        }
        else
        {
            //Debug.LogWarning("No save file found to delete for TimeTrial.");
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
            //Debug.LogWarning("Theme save file not found.");
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
            //Debug.LogWarning("Settings save file not found.");
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
            //Debug.LogWarning("Player progress save file not found.");
            return new PlayerProgressData();
        }

        string json = File.ReadAllText(filePath);
        PlayerProgressWrapper wrapper = JsonUtility.FromJson<PlayerProgressWrapper>(json);
        return wrapper.PlayerData;
    }

    public void SaveGameMetaData(GameMode gameMode, bool isNewGame)
    {
        GameMetaData metaData = new GameMetaData
        {
            CurrentGameMode = gameMode,
            IsNewGame = isNewGame
        };

        string json = JsonUtility.ToJson(metaData, true);
        string filePath = Path.Combine(Application.persistentDataPath, SaveGameMetaDataFileName);
        File.WriteAllText(filePath, json);
        //Debug.Log($"Game MetaData saved for {gameMode}");
    }

    public GameMetaData LoadGameMetaData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, SaveGameMetaDataFileName);
        if (!File.Exists(filePath))
        {
            //Debug.LogWarning("Game MetaData not found, starting fresh.");
            return new GameMetaData();
        }

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<GameMetaData>(json);
    }
}
