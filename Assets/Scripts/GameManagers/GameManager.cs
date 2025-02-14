using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private SimpleInfiniteStatisticController _simpleInfiniteStatisticController;
    [SerializeField] private HardcoreStatisticController _hardcoreStatisticController;
    [SerializeField] private TimeTrialStatisticController _timeTrialStatisticController;
    [SerializeField] private ClassicModeStatisticController _classicStatisticController;
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

    private void Start()
    {
        LoadGameState();
        LoadCustomLevelData();        
    }

    public void SetCustomLevelSettings(LevelConfig customLevel)
    {
        CustomLevel = customLevel;
        SaveManager.Instance.SaveCustomLevel(customLevel);
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

        PlayerProgress.Instance.SetLastSessionGameMode(LastSessionGameMode);
        PlayerProgress.Instance.SetLastClassicSessionGameMode(LastClassicSessionMode);
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
                SaveManager.Instance.ClearSavedSimpleInfiniteGame();
                break;

            case GameMode.Hardcore:
                SaveManager.Instance.ClearSavedHardcoreGame();
                break;

            case GameMode.TimeTrial:
                SaveManager.Instance.ClearSavedTimeTrialGame();
                break;

            case GameMode.ClassicEasy:
            case GameMode.ClassicMedium:
            case GameMode.ClassicHard:
            case GameMode.Custom:
                SaveManager.Instance.ClearSavesClassicGame();
                break;

            default:
                Debug.LogError("Unknown game mode. Cannot save.");
                break;
        }
    }
    
    private void LoadGameState()
    {
        _simpleInfiniteStatisticController.InitializeFromData(SaveManager.Instance.LoadSimpleInfiniteModeStats());
        _hardcoreStatisticController.InitializeFromData(SaveManager.Instance.LoadHardcoreModeStats());
        _timeTrialStatisticController.InitializeFromData(SaveManager.Instance.LoadTimeTrialModeStats());
        _classicStatisticController.InitializeFromData(SaveManager.Instance.LoadClassicModeStats());       
    }

    private void LoadCustomLevelData()
    {
        CustomLevel = SaveManager.Instance.LoadCustomLevel();
    }

    private void OnPlayerProgressLoaded(PlayerProgressLoadCompletedSignal signal)
    {
        LastSessionGameMode = PlayerProgress.Instance.LastSessionGameMode;
        LastClassicSessionMode = PlayerProgress.Instance.LastClassicSessionMode;

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


