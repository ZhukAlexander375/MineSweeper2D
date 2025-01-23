
using System;
using UnityEngine;

public class TimeModeTimerManager : MonoBehaviour
{
    public static TimeModeTimerManager Instance { get; private set; }

    [SerializeField] private TimeTrialSettings _timeTrialSettings;
    public long TimerStartTime { get; private set; }
    public float DurationInSec { get; private set; }
    public bool IsTimerRunning { get; private set; }
    public bool IsTimerOver { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetTimeDuration();
    }    

    private void Start()
    {               
        LoadTimer();
        
        if (!IsTimerRunning && IsTimeUp() && IsTimerOver)
        {
            Debug.Log("Time is up! The game ended while you were away.");
            SignalBus.Fire(
                new GameOverSignal(
                    GameManager.Instance.CurrentGameMode,
                    TimeTrialStatisticController.Instance.IsGameOver,
                    TimeTrialStatisticController.Instance.IsGameWin
                )
            );
        }
    }

    private void Update()
    {
        if (IsTimerRunning && IsTimeUp())
        {
            StopModeTimer();            
        }
    }

    public void StartModeTimer()
    {
        //Debug.Log($"на старте таймера: {TimerStartTime}, {IsTimerRunning}, {IsTimerOver}");
        if (!IsTimerRunning && IsTimerOver)
        {
            //Debug.Log("старт таймер");
            TimerStartTime = GetUnixTimeNow();
            IsTimerRunning = true;
            IsTimerOver = false;
            SaveTimer();
        }
    }

    public void StopModeTimer()
    {        
        if (IsTimerRunning)
        {
            IsTimerRunning = false;
            IsTimerOver = true;

            TimeTrialStatisticController.Instance.IsGameOver = true;

            SignalBus.Fire(
                new GameOverSignal(
                    GameManager.Instance.CurrentGameMode,
                    TimeTrialStatisticController.Instance.IsGameOver,
                    TimeTrialStatisticController.Instance.IsGameWin
                )
            );

            SaveTimer();
        }
    }

    public void ResetModeTimer()
    {
        IsTimerRunning = false;
        IsTimerOver = true;
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
        SaveManager.Instance.SaveTimer(timerManager);
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
        TimerManagerData timerManagerData = SaveManager.Instance.LoadTimerManager();

        if (timerManagerData != null)
        {
            TimerStartTime = timerManagerData.TimerStartTime;
            IsTimerRunning = timerManagerData.IsTimerRunning;
            IsTimerOver= timerManagerData.IsTimerOver;
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
