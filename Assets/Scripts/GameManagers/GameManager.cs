using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [SerializeField] public List<LevelConfig> PredefinedLevels;    
    [SerializeField] private int _hardcoreTimeModeCost;

    public LevelConfig CustomLevel;
    public SimpleInfiniteStatisticController SimpleInfiniteStats => _simpleInfiniteStatisticController;
    public HardcoreStatisticController HardcoreStats => _hardcoreStatisticController;
    public TimeTrialStatisticController TimeTrialStats => _timeTrialStatisticController;
    public ClassicModeStatisticController ClassicStats => _classicStatisticController;
    public int HardcoreTimeModeCost => _hardcoreTimeModeCost;

    public GameMode CurrentGameMode { get; private set; }
    public IStatisticController CurrentStatisticController { get; private set; }
    public GameMode LastSessionGameMode { get; private set; }
    public GameMode LastClassicSessionMode { get; private set; }
    public int CustomWidth { get; private set; }
    public int CustomHeight { get; private set; }
    public int CustomMines { get; private set; }

    private SaveManager _saveManager;
    private PlayerProgress _playerProgress;
    private SimpleInfiniteStatisticController _simpleInfiniteStatisticController;
    private HardcoreStatisticController _hardcoreStatisticController;
    private TimeTrialStatisticController _timeTrialStatisticController;
    private ClassicModeStatisticController _classicStatisticController;

    [Inject]
    private void Construct(
        SaveManager saveManager, 
        PlayerProgress playerProgress, 
        SimpleInfiniteStatisticController simpleInfiniteStatisticController,
        HardcoreStatisticController hardcoreStatisticController,
        TimeTrialStatisticController timeTrialStatisticController,
        ClassicModeStatisticController classicStatisticController)
    {
        _saveManager = saveManager;
        _playerProgress = playerProgress; 

        _simpleInfiniteStatisticController = simpleInfiniteStatisticController;
        _hardcoreStatisticController = hardcoreStatisticController;
        _timeTrialStatisticController = timeTrialStatisticController;
        _classicStatisticController = classicStatisticController;
    }

    private void Start()
    {
        LoadGameState();
        LoadCustomLevelData();        
    }

    public void SetCustomLevelSettings(LevelConfig customLevel)
    {
        CustomLevel = customLevel;
        _saveManager.SaveCustomLevel(customLevel);
    }


    public void SetCurrentGameMode(GameMode mode)
    {
        switch (mode)
        {
            case GameMode.SimpleInfinite:
                CurrentGameMode = GameMode.SimpleInfinite;
                CurrentStatisticController = SimpleInfiniteStats;
                LastSessionGameMode = mode;
                break;

            case GameMode.Hardcore:
                CurrentGameMode = GameMode.Hardcore;
                CurrentStatisticController = HardcoreStats;
                LastSessionGameMode = mode;
                break;

            case GameMode.TimeTrial:
                CurrentGameMode = GameMode.TimeTrial;
                CurrentStatisticController = TimeTrialStats;
                LastSessionGameMode = mode;
                break;

            case GameMode.ClassicEasy:
                CurrentGameMode = GameMode.ClassicEasy;
                CurrentStatisticController = ClassicStats;
                LastSessionGameMode = mode;
                LastClassicSessionMode = mode;
                break;

            case GameMode.ClassicMedium:
                CurrentGameMode = GameMode.ClassicMedium;
                CurrentStatisticController = ClassicStats;
                LastSessionGameMode = mode;
                LastClassicSessionMode = mode;
                break;

            case GameMode.ClassicHard:
                CurrentGameMode = GameMode.ClassicHard;
                CurrentStatisticController = ClassicStats;
                LastSessionGameMode = mode;
                LastClassicSessionMode = mode;
                break;

            case GameMode.Custom:
                CurrentGameMode = GameMode.Custom;
                CurrentStatisticController = ClassicStats;
                LastSessionGameMode = mode;
                LastClassicSessionMode = mode;
                break;
        }

        _playerProgress.SetLastSessionGameMode(LastSessionGameMode);
        _playerProgress.SetLastClassicSessionGameMode(LastClassicSessionMode);
    }

    public void ResetCurrentModeStatistic()
    {
        switch (CurrentGameMode)
        {
            case GameMode.SimpleInfinite:
                _simpleInfiniteStatisticController.ResetStatistic();
                break;
            case GameMode.Hardcore:
                _hardcoreStatisticController.ResetStatistic();
                break;
            case GameMode.TimeTrial:
                _timeTrialStatisticController.ResetStatistic();
                break;
            case GameMode.ClassicEasy:
                _classicStatisticController.ResetStatistic();
                break;
            case GameMode.ClassicMedium:
                _classicStatisticController.ResetStatistic();
                break;
            case GameMode.ClassicHard:
                _classicStatisticController.ResetStatistic();
                break;
            case GameMode.Custom:
                _classicStatisticController.ResetStatistic();
                break;
            default:
                Debug.LogWarning("Unknown game mode.");
                break;
        }
    }

    public void ClearCurrentGame(GameMode mode)
    {
        switch (mode)
        {
            case GameMode.SimpleInfinite:
                _saveManager.ClearSavedSimpleInfiniteGame();
                break;

            case GameMode.Hardcore:
                _saveManager.ClearSavedHardcoreGame();
                break;

            case GameMode.TimeTrial:
                _saveManager.ClearSavedTimeTrialGame();
                break;

            case GameMode.ClassicEasy:
            case GameMode.ClassicMedium:
            case GameMode.ClassicHard:
            case GameMode.Custom:
                _saveManager.ClearSavesClassicGame();
                break;

            default:
                Debug.LogError("Unknown game mode. Cannot save.");
                break;
        }
    }
    
    private void LoadGameState()
    {
        _simpleInfiniteStatisticController.InitializeFromData(_saveManager.LoadSimpleInfiniteModeStats());
        _hardcoreStatisticController.InitializeFromData(_saveManager.LoadHardcoreModeStats());
        _timeTrialStatisticController.InitializeFromData(_saveManager.LoadTimeTrialModeStats());
        _classicStatisticController.InitializeFromData(_saveManager.LoadClassicModeStats());       
    }

    private void LoadCustomLevelData()
    {
        CustomLevel = _saveManager.LoadCustomLevel();
    }

    private void OnPlayerProgressLoaded(PlayerProgressLoadCompletedSignal signal)
    {
        LastSessionGameMode = _playerProgress.LastSessionGameMode;
        LastClassicSessionMode = _playerProgress.LastClassicSessionMode;

        switch (LastSessionGameMode)
        {
            case GameMode.SimpleInfinite:
                {
                    CurrentStatisticController = _simpleInfiniteStatisticController;
                    break;
                }

            case GameMode.Hardcore:
                {
                    CurrentStatisticController = _hardcoreStatisticController;
                    break;
                }
            case GameMode.TimeTrial:
                {
                    CurrentStatisticController = _timeTrialStatisticController;
                    break;
                }

            case GameMode.ClassicEasy:
            case GameMode.ClassicMedium:
            case GameMode.ClassicHard:
            case GameMode.Custom:
                {
                    CurrentStatisticController = _classicStatisticController;
                    break;
                }
        }

        if (CurrentStatisticController != null)
        {
            SignalBus.Fire<GameManagerLoadCompletedSignal>();
        }
    }

    private void OnEnable()
    {
        SignalBus.Subscribe<PlayerProgressLoadCompletedSignal>(OnPlayerProgressLoaded);
    }
    private void OnDestroy()
    {        
        SignalBus.Unsubscribe<PlayerProgressLoadCompletedSignal>(OnPlayerProgressLoaded);
    }
}


