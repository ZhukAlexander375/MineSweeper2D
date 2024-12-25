
[System.Serializable]

public class PlayerProgressData
{
    public int TotalReward;
    public GameMode LastSessionGameMode;
   
    //public int TotalPlacedFlags;
    //public int TotalOpenedCells;
    //public int TotalCompletedSectors;

    public PlayerProgressData(PlayerProgressData playerProgress = null)
    {
        if (playerProgress != null)
        {
            TotalReward = playerProgress.TotalReward;
            LastSessionGameMode = playerProgress.LastSessionGameMode;
        }
    }
}
