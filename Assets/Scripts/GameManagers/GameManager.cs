
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameMode CurrentGameMode { get; private set; }
    public IGameModeData CurrentGameModeData { get; private set; }
    public GameMode LastPlayedMode { get; private set; }

    private Dictionary<GameMode, IGameModeData> _gameModeData = new();

   
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
        //LoadGameState();
    }

    public void SetGameModeData(GameMode mode, IGameModeData data)
    {
        if (_gameModeData.ContainsKey(mode))
        {
            _gameModeData[mode] = data;
        }
        else
        {
            _gameModeData.Add(mode, data);
        }
    }

    public IGameModeData GetGameModeData(GameMode mode)
    {
        if (_gameModeData.TryGetValue(mode, out var data))
        {
            return data;
        }

        return null;
        
    }

    public void SetCurrentGameMode(GameMode mode, bool isNewGame = true)
    {
        switch (mode)
        {
            case GameMode.SimpleInfinite:
                CurrentGameMode = GameMode.SimpleInfinite;
                CurrentGameModeData = new SimpleInfiniteModeData();
                break;

            case GameMode.Hardcore:
                CurrentGameMode = GameMode.Hardcore;
                CurrentGameModeData = new HardcoreModeData();
                break;

            case GameMode.TimeTrial:
                CurrentGameMode = GameMode.TimeTrial;
                CurrentGameModeData = new TimeTrialModeData();
                break;
        }

        CurrentGameModeData.IsGameStarted = isNewGame;
        LastPlayedMode = mode;
    }

    public void SaveGameModes()
    {
        if (CurrentGameModeData != null)
        {
            SaveManager.Instance.SaveGameMetaData(CurrentGameMode, CurrentGameModeData.IsGameStarted);
        }
    }

    public void ApplyGameModeData(IGameModeData gameModeData)
    {
        CurrentGameModeData = gameModeData;

        switch (gameModeData)
        {
            case SimpleInfiniteModeData simpleData:
                simpleData.InitializeFromSave(simpleData);
                break;

            case HardcoreModeData hardcoreData:
                hardcoreData.InitializeFromSave(hardcoreData);
                break;

            case TimeTrialModeData timeTrialData:
                timeTrialData.InitializeFromSave(timeTrialData);
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


