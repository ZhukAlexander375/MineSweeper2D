
public interface IStatisticController
{
    public bool IsGameStarted { get; set; }
    public int OpenedCells { get; set; }
    public int PlacedFlags { get; set; }
    public int CompletedSectors { get; set; }
    public int ExplodedMines { get; set; }
    public int RewardLevel { get; set; }
    public int SectorBuyoutCostLevel { get; set; }
    float TotalPlayTime { get; set; }
    void IncrementOpenedCells();
    void IncrementPlacedFlags(bool isPlacingFlag);
    void IncrementCompletedSectors();    
    void IncrementExplodedMines();
    void IncrementRewardLevel();
    void IncrementSectorBuyoutIndex();
    void ResetStatistic();
    void StartTimer();
    void StopTimer();
}
