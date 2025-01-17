using System;
using UnityEngine;

public class ClassicModeStatisticController : MonoBehaviour, IStatisticController
{
    public ClassicModeStatisticController Instance { get; private set; }

    public bool IsGameStarted { get; set; }
    public int OpenedCells { get; set; }
    public int PlacedFlags { get; set; }
    public int ExplodedMines { get; set; }
    public float TotalPlayTime { get; set; }
    public bool IsGameOver { get; set; }
    public bool IsGameWin { get; set;} 
    public int CompletedSectors { get; set; }       // interface
    public int RewardLevel { get; set; }            // interface
    public int SectorBuyoutCostLevel { get; set; }  // interface
    public Vector3 LastClickPosition { get; set; }  // interface

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

    public void InitializeFromData(ClassicModeData data)
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
        IsGameWin = data.IsGameWin;
    }

    public void ResetStatistic()
    {
        IsGameStarted = false;
        OpenedCells = 0;
        PlacedFlags = 0;
        ExplodedMines = 0;

        TotalPlayTime = 0;
        _isTimerRunning = false;
        _sessionStartTime = 0;
        IsGameOver = false;
        IsGameWin = false;
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

    public void IncrementExplodedMines()
    {
        ExplodedMines++;
    }

    public void SetLastClickPosition(Vector3 position)
    {
        LastClickPosition = position;
    }

    public void IncrementCompletedSectors()
    {        
    }

    public void IncrementRewardLevel()
    {        
    }

    public void IncrementSectorBuyoutIndex()
    {       
    }
}
