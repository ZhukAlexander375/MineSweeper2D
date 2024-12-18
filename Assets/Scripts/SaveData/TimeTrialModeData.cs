
[System.Serializable]
public class TimeTrialModeData :IGameModeData
{
    public GameMode Mode => GameMode.TimeTrial;
    public bool IsGameStarted { get; set; }
    public int OpenedCells;
    public int PlacedFlags;
    public int CompletedSectors;
    public int ExplodedMines;
    public int RewardLevel;
    public int SectorBuyoutCostLevel;

    public void InitializeNewGame()
    {
        OpenedCells = 0;
        PlacedFlags = 0;
        CompletedSectors = 0;
        ExplodedMines = 0;
        RewardLevel = 0;
        SectorBuyoutCostLevel = 0;
    }

    public void InitializeFromSave(IGameModeData savedData)
    {
        if (savedData is not TimeTrialModeData timeTrialData)
        {
            return;
        }

        IsGameStarted = timeTrialData.IsGameStarted;
        OpenedCells = timeTrialData.OpenedCells;
        PlacedFlags = timeTrialData.PlacedFlags;
        CompletedSectors = timeTrialData.CompletedSectors;
        ExplodedMines = timeTrialData.ExplodedMines;
        RewardLevel = timeTrialData.RewardLevel;
        SectorBuyoutCostLevel = timeTrialData.SectorBuyoutCostLevel;
    }

    public void IncrementOpenedCells()
    {
        OpenedCells++;
    }

    public void IncrementCompletedSectors()
    {
        CompletedSectors++;
    }

    public void IncrementPlacedFlags(bool isPlacingFlag)
    {
        if (isPlacingFlag)
        {
            PlacedFlags++;
        }
        else if (!isPlacingFlag)
        {
            PlacedFlags--;

            if (PlacedFlags < 0)
            {
                PlacedFlags = 0;
            }
        }
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

    public int GetOpenedCells()
    {
        return OpenedCells;
    }

    public int GetPlacedFlags()
    {
        return PlacedFlags;
    }
    public int GetCompletedSectors()
    {
        return CompletedSectors;
    }

    public int GetExplodedMines()
    {
        return ExplodedMines;
    }

    public int GetRewardLevel()
    {
        return RewardLevel;
    }

    public int GetSectorBuyoutLevel()
    {
        return SectorBuyoutCostLevel;
    }
}
