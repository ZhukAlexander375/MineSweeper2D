using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private SimpleInfiniteStatisticController _simpleInfiniteStatisticController;
    [SerializeField] private HardcoreStatisticController _hardcoreStatisticController;
    [SerializeField] private TimeTrialStatisticController _timeTrialStatisticController;
    public SimpleInfiniteStatisticController SimpleInfiniteStats => _simpleInfiniteStatisticController;
    public HardcoreStatisticController HardcoreStats => _hardcoreStatisticController;
    public TimeTrialStatisticController TimeTrialStats => _timeTrialStatisticController;
    
    public GameMode CurrentGameMode { get; private set; }
    public IStatisticController CurrentStatisticController { get; private set; }
    public GameMode LastPlayedMode { get; private set; }

   

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
        SignalBus.Fire<LoadCompletedSignal>();
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
        }

        LastPlayedMode = mode;
    }

    /*public void SetCurrentGameMode(GameMode mode)
    {
        CurrentGameMode = mode;        
    }*/

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
            default:
                Debug.LogWarning("Unknown game mode.");
                break;
        }
    }

    public void SaveGameModes()
    {
        SaveManager.Instance.SaveGameMetaData(CurrentGameMode, true);        
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

            default:
                Debug.LogError("Unknown game mode. Cannot save.");
                break;
        }
    }   

    public void ContinueGame()
    {
        
    }

    private void OnApplicationQuit()
    {
        SaveGameModes();
    }
}


