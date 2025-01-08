using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private SimpleInfiniteStatisticController _simpleInfiniteStatisticController;
    [SerializeField] private HardcoreStatisticController _hardcoreStatisticController;
    [SerializeField] private TimeTrialStatisticController _timeTrialStatisticController;
    [SerializeField] private ClassicModeStatisticController _classicStatisticController;
    public SimpleInfiniteStatisticController SimpleInfiniteStats => _simpleInfiniteStatisticController;
    public HardcoreStatisticController HardcoreStats => _hardcoreStatisticController;
    public TimeTrialStatisticController TimeTrialStats => _timeTrialStatisticController;
    public ClassicModeStatisticController ClassicStats => _classicStatisticController;

    public GameMode CurrentGameMode { get; private set; }
    public IStatisticController CurrentStatisticController { get; private set; }
    public GameMode LastSessionGameMode { get; private set; }
    public GameMode LastClassicSessionMode { get; private set; }



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
    }

    private void LoadGameState()
    {
        _simpleInfiniteStatisticController.InitializeFromData(SaveManager.Instance.LoadSimpleInfiniteModeStats());
        _hardcoreStatisticController.InitializeFromData(SaveManager.Instance.LoadHardcoreModeStats());
        _timeTrialStatisticController.InitializeFromData(SaveManager.Instance.LoadTimeTrialModeStats());
        _classicStatisticController.InitializeFromData(SaveManager.Instance.LoadClassicModeStats());
               
        LastSessionGameMode = PlayerProgress.Instance.LastSessionGameMode;
        LastClassicSessionMode = PlayerProgress.Instance.LastClassicSessionMode;

        switch (LastSessionGameMode)
        {
            case (GameMode.SimpleInfinite):
            {
                CurrentStatisticController = _simpleInfiniteStatisticController;
                break;
            }

            case (GameMode.Hardcore):
            {
                CurrentStatisticController = _hardcoreStatisticController;
                break;
            }
            case (GameMode.TimeTrial):
            {
                CurrentStatisticController = _timeTrialStatisticController;
                break;
            }

            case (GameMode.ClassicEasy):
            case (GameMode.ClassicMedium):
            case (GameMode.ClassicHard):
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

    // MB START NEW GAME????
    public void SetCurrentGameMode(GameMode mode, bool isNewGame = true)
    {
        switch (mode)
        {
            case GameMode.SimpleInfinite:
                CurrentGameMode = GameMode.SimpleInfinite;
                CurrentStatisticController = SimpleInfiniteStats;
                SimpleInfiniteStats.IsGameStarted = isNewGame;
                break;

            case GameMode.Hardcore:
                CurrentGameMode = GameMode.Hardcore;
                CurrentStatisticController = HardcoreStats;
                HardcoreStats.IsGameStarted = isNewGame;
                break;

            case GameMode.TimeTrial:
                CurrentGameMode = GameMode.TimeTrial;
                CurrentStatisticController = TimeTrialStats;
                TimeTrialStats.IsGameStarted = isNewGame;
                break;

            case GameMode.ClassicEasy:
                CurrentGameMode = GameMode.ClassicEasy;
                CurrentStatisticController = ClassicStats;
                ClassicStats.IsGameStarted = isNewGame;
                LastClassicSessionMode = mode;
                break;

            case GameMode.ClassicMedium:
                CurrentGameMode = GameMode.ClassicMedium;
                CurrentStatisticController = ClassicStats;
                ClassicStats.IsGameStarted = isNewGame;
                LastClassicSessionMode = mode;
                break;

            case GameMode.ClassicHard:
                CurrentGameMode = GameMode.ClassicHard;
                CurrentStatisticController = ClassicStats;
                ClassicStats.IsGameStarted = isNewGame;
                LastClassicSessionMode = mode;
                break;
        }

        LastSessionGameMode = mode;
       
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
            default:
                Debug.LogWarning("Unknown game mode.");
                break;
        }
    }

    public void SaveGameModes()
    {
        //SaveManager.Instance.SaveGameMetaData(CurrentGameMode, true);        
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
                SaveManager.Instance.ClearSavesClassicGame();
                break;

            default:
                Debug.LogError("Unknown game mode. Cannot save.");
                break;
        }
    }

    private void OnApplicationQuit()
    {
        //SaveGameModes();
    }

    private void GameOver(GameOverSignal signal)
    {

    }

    private void OnEnable()
    {
        SignalBus.Subscribe<GameOverSignal>(GameOver);
    }

    private void OnDisable()
    {
        SignalBus.Unsubscribe<GameOverSignal>(GameOver);
    }
}


