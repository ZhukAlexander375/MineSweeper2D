using UnityEngine;

public class HardcoreStatisticController : MonoBehaviour, IStatisticController
{
    public static HardcoreStatisticController Instance { get; private set; }

    public bool IsGameStarted { get; set; }
    public int OpenedCells { get; set; }
    public int PlacedFlags { get; set; }
    public int CompletedSectors { get; set; }
    public int ExplodedMines { get; set; }
    public int RewardLevel { get; set; }
    public int SectorBuyoutCostLevel { get; set; }
    public float TotalPlayTime { get; set; }
    public bool IsGameOver { get; set; }

    private float _sessionStartTime;
    private bool _isTimerRunning;

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

    public void InitializeFromData(HardcoreModeData data)
    {
        IsGameStarted = data.IsGameStarted;
        OpenedCells = data.OpenedCells;
        PlacedFlags = data.PlacedFlags;
        CompletedSectors = data.CompletedSectors;
        ExplodedMines = data.ExplodedMines;
        RewardLevel = data.RewardLevel;
        SectorBuyoutCostLevel = data.SectorBuyoutCostLevel;
        TotalPlayTime = data.TotalPlayTime;
        IsGameOver = data.IsGameOver;
    }

    public void ResetStatistic()
    {
        IsGameStarted = false;
        OpenedCells = 0;
        PlacedFlags = 0;
        CompletedSectors = 0;
        ExplodedMines = 0;
        RewardLevel = 0;
        SectorBuyoutCostLevel = 0;

        TotalPlayTime = 0;
        _isTimerRunning = false;
        _sessionStartTime = 0;
        IsGameOver = false;
    }

    public void StartTimer()
    {
        if (!_isTimerRunning && !IsGameOver)
        {
            _sessionStartTime = Time.time;
            _isTimerRunning = true;
        }
        else
        {
            //Debug.Log("запустить таймер");
        }
    }

    public void StopTimer()
    {
        if (_isTimerRunning)
        {
            float sessionDuration = Time.time - _sessionStartTime;
            TotalPlayTime += sessionDuration;
            _isTimerRunning = false;
        }
        else
        {
            //Debug.Log("остановить таймер");
        }
    }

    public void IncrementOpenedCells()
    {
        OpenedCells++;
    }

    public void IncrementPlacedFlags(bool isPlacingFlag)
    {
        if (isPlacingFlag)
        {
            PlacedFlags++;
        }
        else
        {
            PlacedFlags--;
        }
    }

    public void IncrementCompletedSectors()
    {
        CompletedSectors++;
    }

    public void IncrementExplodedMines()
    {
        ExplodedMines++;
    }

    public void IncrementRewardLevel()
    {
        RewardLevel++;
    }

    public void IncrementSectorBuyoutIndex()
    {
        SectorBuyoutCostLevel++;
    }
}
