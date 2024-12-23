using UnityEngine;

public class TimeTrialStatisticController : MonoBehaviour, IStatisticController
{
    public static TimeTrialStatisticController Instance { get; private set; }

    public bool IsGameStarted { get; set; }
    public int OpenedCells { get; set; }
    public int PlacedFlags { get; set; }
    public int CompletedSectors { get; set; }
    public int ExplodedMines { get; set; }
    public int RewardLevel { get; set; }
    public int SectorBuyoutCostLevel { get; set; }

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

    public void InitializeFromData(TimeTrialModeData data)
    {
        IsGameStarted = data.IsGameStarted;
        OpenedCells = data.OpenedCells;
        PlacedFlags = data.PlacedFlags;
        CompletedSectors = data.CompletedSectors;
        ExplodedMines = data.ExplodedMines;
        RewardLevel = data.RewardLevel;
        SectorBuyoutCostLevel = data.SectorBuyoutCostLevel;
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
