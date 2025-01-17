using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
        
    private const string SaveThemeFileName = "Theme.json";
    private const string SaveSettingsFileName = "GameSettings.json";
    private const string SavePlayerProgressFileName = "PlayerProgress.json";

    private const string SaveSimpleInfiniteGameFileName = "SimpleInfiniteGameSave.json";
    private const string SaveHardcoreGameFileName = "HardcoreInfiniteGameSave.json";
    private const string SaveTimeTrialGameFileName = "TimeTrialInfiniteGameSave.json";
    private const string SaveClassicGameFileName = "ClassicGameSave.json";

    private const string SaveTimerFileName = "TimerSave.json";

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

    public bool HasClassicGameSave()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveClassicGameFileName);
        return File.Exists(filePath);
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
                TotalPlayTime = simpleInfiniteStats.TotalPlayTime,
                IsGameOver = simpleInfiniteStats.IsGameOver,
                IsGameWin = simpleInfiniteStats.IsGameWin,
                LastClickPosition = simpleInfiniteStats.LastClickPosition,
            }
        };
        string json = JsonUtility.ToJson(saveData, true);
        var filePath = Path.Combine(Application.persistentDataPath, SaveSimpleInfiniteGameFileName);
        File.WriteAllText(filePath, json);
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
                TotalPlayTime = hardcoreStats.TotalPlayTime,
                IsGameOver = hardcoreStats.IsGameOver,
                IsGameWin = hardcoreStats.IsGameWin,
                LastClickPosition = hardcoreStats.LastClickPosition,
            }
        };
        string json = JsonUtility.ToJson(saveData, true);
        var filePath = Path.Combine(Application.persistentDataPath, SaveHardcoreGameFileName);
        File.WriteAllText(filePath, json);
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
                TotalPlayTime = timeTrialStats.TotalPlayTime,
                IsGameOver = timeTrialStats.IsGameOver,
                IsGameWin = timeTrialStats.IsGameWin,
                LastClickPosition = timeTrialStats.LastClickPosition
            }
        };
        string json = JsonUtility.ToJson(saveData, true);
        var filePath = Path.Combine(Application.persistentDataPath, SaveTimeTrialGameFileName);
        File.WriteAllText(filePath, json);
    }

    public void SaveClassicGame(CellGrid cellGrid, ClassicModeStatisticController classicStats)
    {
        ClassicGameSaveWrapper saveData = new ClassicGameSaveWrapper
        {
            SimpleGridData = new SimpleGridData
            {
                Width = cellGrid.Width,
                Height = cellGrid.Height,
                Cells = cellGrid.GetAllCells().Select(cell => new CellData
                {
                    GlobalCellPosition = cell.GlobalCellPosition,
                    CellState = cell.CellState,                    
                    IsRevealed = cell.IsRevealed,
                    IsFlagged = cell.IsFlagged,
                    IsExploded = cell.IsExploded,
                    Chorded = cell.Chorded,
                    CellNumber = cell.CellNumber
                }).ToList()
            },
            
            ClassicModeData = new ClassicModeData(classicStats)
            {
                IsGameStarted = classicStats.IsGameStarted,
                OpenedCells = classicStats.OpenedCells,
                PlacedFlags = classicStats.PlacedFlags,
                CompletedSectors = classicStats.CompletedSectors,
                ExplodedMines = classicStats.ExplodedMines,
                RewardLevel = classicStats.RewardLevel,
                SectorBuyoutCostLevel = classicStats.SectorBuyoutCostLevel,
                TotalPlayTime = classicStats.TotalPlayTime,
                IsGameOver = classicStats.IsGameOver,
                IsGameWin = classicStats.IsGameWin,
            }
        };
        string json = JsonUtility.ToJson(saveData, true);
        var filePath = Path.Combine(Application.persistentDataPath, SaveClassicGameFileName);
        File.WriteAllText(filePath, json);
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

        //Debug.Log($"загружено из {filePath}");
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
        
        //Debug.Log($"Hardcore Game stats loaded successfully from {saveData.HardcoreModeData.OpenedCells}");
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
        
        //Debug.Log($"Simple Infinite Game stats loaded successfully from {filePath}");
        return saveData.TimeTrialModeData;
    }

    public ClassicModeData LoadClassicModeStats()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveClassicGameFileName);

        if (!File.Exists(filePath))
        {
            return new ClassicModeData();
        }

        string json = File.ReadAllText(filePath);
        ClassicGameSaveWrapper saveData = JsonUtility.FromJson<ClassicGameSaveWrapper>(json);

        return saveData.ClassicModeData;
    }

    /// <summary>
    /// FOR LOAD ONLY MODES GRIDS 
    /// </summary>

    public List<SectorData> LoadSimpleInfiniteGameGrid()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveSimpleInfiniteGameFileName);

        if (!File.Exists(filePath))
        {
            return null;
        }

        string json = File.ReadAllText(filePath);
        SimpleInfiniteGameSaveWrapper saveData = JsonUtility.FromJson<SimpleInfiniteGameSaveWrapper>(json);
        
        return saveData.Sectors;
    }

    public List<SectorData> LoadHardcoreGameGrid()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveHardcoreGameFileName);

        if (!File.Exists(filePath))
        {
            return null;
        }

        string json = File.ReadAllText(filePath);
        HardcoreGameSaveWrapper saveData = JsonUtility.FromJson<HardcoreGameSaveWrapper>(json);
        
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

        //Debug.Log($"Simple Infinite Game sectors loaded successfully from {filePath}");
        return saveData.Sectors;
    }

    public (SimpleGridData, ClassicModeData) LoadClassicGame()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveClassicGameFileName);
        if (!File.Exists(filePath))
        {
            return (null, null);
        }

        string json = File.ReadAllText(filePath);
        ClassicGameSaveWrapper saveData = JsonUtility.FromJson<ClassicGameSaveWrapper>(json);
        return (saveData.SimpleGridData, saveData.ClassicModeData);
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

        //Debug.Log($"Simple Infinite Game loaded successfully from {filePath}");
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

    public void ClearSavesClassicGame()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveClassicGameFileName);

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
    
    public void SaveTimer(TimerManagerData timerData)
    {
        TimerManagerWrapper wrapper = new TimerManagerWrapper();
        wrapper.TimerManagerData = timerData;

        string json = JsonUtility.ToJson(wrapper, true);
        var filePath = Path.Combine(Application.persistentDataPath, SaveTimerFileName);
        File.WriteAllText(filePath, json);
    }

    public TimerManagerData LoadTimerManager()
    {
        var filePath = Path.Combine(Application.persistentDataPath, SaveTimerFileName);
        if (!File.Exists(filePath))
        {
            return new TimerManagerData();
        }

        string json = File.ReadAllText(filePath);
        TimerManagerWrapper wrapper = JsonUtility.FromJson<TimerManagerWrapper>(json);
        return wrapper.TimerManagerData;
    }
}
