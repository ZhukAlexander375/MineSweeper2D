using System;
using UnityEngine;
using Zenject;

public class TimeModeTimerManager : MonoBehaviour
{
    public long TimerStartTime { get; private set; }
    public float DurationInSec { get; private set; }
    public bool IsTimerRunning { get; private set; }
    public bool IsTimerOver { get; private set; }
        
    private SaveManager _saveManager;
    private GameManager _gameManager;
    private TimeTrialSettings _timeTrialSettings;

    [Inject]
    private void Construct(SaveManager saveManager, GameManager gameManager, TimeTrialSettings timeTrialSettings)
    {
        _saveManager = saveManager;
        _gameManager = gameManager;
        _timeTrialSettings = timeTrialSettings;
    }

    private void Start()
    {
        SetTimeDuration();        
        LoadTimer();

        if (!IsTimerRunning && IsTimeUp() && IsTimerOver)
        {
            SignalBus.Fire(
                new GameOverSignal(
                    _gameManager.CurrentGameMode,
                    _gameManager.TimeTrialStats.IsGameOver,
                    _gameManager.TimeTrialStats.IsGameWin
                )
            );
        }
    }

    private void FixedUpdate()
    {
        if (IsTimerRunning && IsTimeUp())
        {
            StopModeTimer();            
        }
    }

    public void StartModeTimer()
    {
        if (IsTimerRunning)
        {            
            return;
        }

        if (IsTimerOver)
        {
            ResetModeTimer();
        }

        TimerStartTime = GetUnixTimeNow();
        IsTimerRunning = true;
        IsTimerOver = false;
        SaveTimer();
        
    }

    public void StopModeTimer()
    {        
        if (IsTimerRunning)
        {
            IsTimerRunning = false;
            IsTimerOver = true;

            _gameManager.TimeTrialStats.IsGameOver = true;

            SignalBus.Fire(
                new GameOverSignal(
                    _gameManager.CurrentGameMode,
                    _gameManager.TimeTrialStats.IsGameOver,
                    _gameManager.TimeTrialStats.IsGameWin
                )
            );

            SaveTimer();
        }
    }

    public void ResetModeTimer()
    {
        IsTimerRunning = false;
        IsTimerOver = false;
        TimerStartTime = GetUnixTimeNow();
        SaveTimer();
    }

    public float GetRemainingTime()
    {
        if (!IsTimerRunning)
        {
            return DurationInSec;
        }

        long currentUnixTime = GetUnixTimeNow();
        return Mathf.Max(0, DurationInSec - (currentUnixTime - TimerStartTime));
    }

    public bool IsTimeUp()
    {
        return GetRemainingTime() <= 0;
    }

    public void SaveTimer()
    {
        TimerManagerData timerManager = new TimerManagerData
        {           
            TimerStartTime = TimerStartTime,
            IsTimerRunning = IsTimerRunning,
            IsTimerOver = IsTimerOver
        };
        //Debug.Log($"сохранение: {TimerStartTime}, {IsTimerRunning}, {IsTimerOver}");
        _saveManager.SaveTimer(timerManager);
    }

    private void SetTimeDuration()
    {
        if (_timeTrialSettings == null)
        {
            return;
        }

        DurationInSec = _timeTrialSettings.DurationInSec;
    }

    private void LoadTimer()
    {
        TimerManagerData timerManagerData = _saveManager.LoadTimerManager();

        if (timerManagerData != null)
        {
            TimerStartTime = timerManagerData.TimerStartTime;
            IsTimerRunning = timerManagerData.IsTimerRunning;
            IsTimerOver = timerManagerData.IsTimerOver;
        }

        if (IsTimeUp())
        {
            IsTimerRunning = false;
            IsTimerOver = true;
            _gameManager.TimeTrialStats.IsGameOver = true;

            //Debug.Log("Time is up! The game ended while you were away.");

            SignalBus.Fire(
                new GameOverSignal(
                    _gameManager.CurrentGameMode,
                    _gameManager.TimeTrialStats.IsGameOver,
                    _gameManager.TimeTrialStats.IsGameWin
                )
            );

            SaveTimer();
        }
    }

    public long GetUnixTimeNow()
    {
        return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
    }

    private void OnApplicationQuit()
    {
        SaveTimer();
    }
}
